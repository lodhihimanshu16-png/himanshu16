using LearnHub.Api.Services;

namespace LearnHub.Tests;

// Unit tests of the pure refund rules. No database and no clock: the dates are passed in directly.
public class RefundPolicyTests
{
    private static readonly DateTime Created = TestData.Today;
    private static readonly (string, double)[] UnderThirty = { ("Intro to C# Programming", 0), ("Joins", 29) };

    [Fact]
    public void Window_ExactlySevenDays_IsAllowed()
    {
        Assert.Null(RefundPolicy.Evaluate(Created, Created.AddDays(7), new[] { ("Course", 0.0) }));
    }

    [Fact]
    public void Window_SevenDaysAndOneMinute_IsRefused()
    {
        var error = RefundPolicy.Evaluate(Created, Created.AddDays(7).AddMinutes(1), new[] { ("Course", 0.0) });
        Assert.Equal("Refunds are only possible within 7 days of the order.", error);
    }

    [Fact]
    public void Progress_ExactlyThirtyPercent_IsRefused()
    {
        var error = RefundPolicy.Evaluate(Created, Created, new[] { ("Data Analysis with SQL", 30.0) });
        Assert.Equal("You have finished 30% of Data Analysis with SQL. Refunds need progress under 30% on every course.", error);
    }

    [Fact]
    public void Progress_TwentyNinePercent_IsAllowed()
    {
        Assert.Null(RefundPolicy.Evaluate(Created, Created, new[] { ("Data Analysis with SQL", 29.0) }));
    }

    [Fact]
    public void MultiCourse_OneAtFortyPercent_AndOneAtZero_IsRefused_AndNamesTheOffendingCourse()
    {
        var courses = new[] { ("JavaScript for the Web", 0.0), ("Data Analysis with SQL", 40.0) };

        var error = RefundPolicy.Evaluate(Created, Created, courses);

        Assert.NotNull(error);
        Assert.Contains("Data Analysis with SQL", error);
        Assert.DoesNotContain("JavaScript", error);
        Assert.Contains("40%", error);
    }

    [Fact]
    public void MultiCourse_AllCoursesUnderThirty_IsAllowed()
    {
        Assert.Null(RefundPolicy.Evaluate(Created, Created, UnderThirty));
    }

    [Fact]
    public void NoCourses_WithinWindow_IsAllowed()
    {
        Assert.Null(RefundPolicy.Evaluate(Created, Created.AddDays(1), Array.Empty<(string, double)>()));
    }
}
