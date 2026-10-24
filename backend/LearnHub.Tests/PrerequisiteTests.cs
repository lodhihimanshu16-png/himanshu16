using LearnHub.Api.Models;

namespace LearnHub.Tests;

public class PrerequisiteTests
{
    private static TestEnv WithPath()
    {
        var env = new TestEnv();
        // Cloud Fundamentals (5, free) must be finished before Intro to C# (1, free).
        env.Db.CoursePrerequisites.Add(new CoursePrerequisite { CourseId = 1, RequiresCourseId = 5 });
        env.Db.SaveChanges();
        return env;
    }

    [Fact]
    public void DirectEnroll_IsBlocked_UntilThePrerequisiteIsFinished()
    {
        using var env = WithPath();
        var blocked = env.Enrollments.EnrollFree(1, "Asha");
        Assert.Null(blocked.Enrollment);
        Assert.Equal("Finish Cloud Fundamentals before you enroll in this course.", blocked.Error);

        env.Enrollments.Enroll(5, "Asha");
        env.CompleteLessons("Asha", 5, 3); // 3 of 4 lessons
        Assert.NotNull(env.Enrollments.EnrollFree(1, "Asha").Error);

        env.CompleteLessons("Asha", 5, 4);
        Assert.NotNull(env.Enrollments.EnrollFree(1, "Asha").Enrollment);
    }

    [Fact]
    public void CourseDetail_ListsPrerequisites()
    {
        using var env = WithPath();
        var prereq = Assert.Single(env.Courses.Get(1)!.Prerequisites);
        Assert.Equal((5, "Cloud Fundamentals"), (prereq.Id, prereq.Title));
        Assert.Empty(env.Courses.Get(5)!.Prerequisites);
    }

    private const string PaidBlockedError = "Finish Cloud Fundamentals before you enroll in this course.";

    /// <summary>Paid course 6 (Containers, 60) requires free course 5 (Cloud Fundamentals).</summary>
    private static TestEnv WithPaidPath()
    {
        var env = new TestEnv();
        env.Db.CoursePrerequisites.Add(new CoursePrerequisite { CourseId = 6, RequiresCourseId = 5 });
        env.Db.SaveChanges();
        return env;
    }

    /// <summary>Puts a cart row straight into the database, bypassing the AddToCart rule.</summary>
    private static void PutInCart(TestEnv env, string name, int courseId)
    {
        var student = env.Students.GetOrCreate(name);
        env.Db.CartItems.Add(new CartItem { StudentId = student.Id, CourseId = courseId, AddedAt = TestData.Today });
        env.Db.SaveChanges();
    }

    [Fact]
    public void Checkout_PaidCourse_IsBlocked_WhenPrerequisiteNotFinished()
    {
        using var env = WithPaidPath();

        var refused = env.Shop.AddToCart("Chidi", 6);
        Assert.Null(refused.Item);
        Assert.Equal(PaidBlockedError, refused.Error);
        Assert.Empty(env.Shop.GetCart("Chidi"));
        Assert.Null(env.Students.Find("Chidi"));

        PutInCart(env, "Asha", 6);
        var before = env.CouponUses("WELCOME10");
        var (order, error) = env.Shop.Checkout("Asha", "WELCOME10");
        Assert.Null(order);
        Assert.Equal(PaidBlockedError, error);
        Assert.Empty(env.Db.Orders);
        Assert.Empty(env.Db.Enrollments.Where(e => e.CourseId == 6));
        Assert.Equal(before, env.CouponUses("WELCOME10"));
    }

    [Fact]
    public void PartialPrerequisite_ThreeOfFourLessons_IsStillBlocked()
    {
        using var env = WithPaidPath();
        env.Enrollments.Enroll(5, "Asha");
        env.CompleteLessons("Asha", 5, 3);

        var refused = env.Shop.AddToCart("Asha", 6);
        Assert.Null(refused.Item);
        Assert.Equal(PaidBlockedError, refused.Error);

        PutInCart(env, "Asha", 6);
        var (order, error) = env.Shop.Checkout("Asha", null);
        Assert.Null(order);
        Assert.Equal(PaidBlockedError, error);
        Assert.Empty(env.Db.Orders);
        Assert.False(env.Enrollments.IsEnrolled("Asha", 6));
    }

    [Fact]
    public void FullPrerequisite_FourOfFourLessons_AllowsAddToCartAndCheckout()
    {
        using var env = WithPaidPath();
        env.Enrollments.Enroll(5, "Asha");
        env.CompleteLessons("Asha", 5, 4);

        Assert.NotNull(env.Shop.AddToCart("Asha", 6).Item);
        var (order, error) = env.Shop.Checkout("Asha", null);
        Assert.Null(error);
        Assert.NotNull(order);
        Assert.Single(env.Db.Orders);
        Assert.True(env.Enrollments.IsEnrolled("Asha", 6));
    }

    [Fact]
    public void Enroll_RawPath_IsBlocked_UntilPrerequisiteFinished()
    {
        using var env = WithPaidPath();

        var blocked = env.Enrollments.Enroll(6, "Asha");
        Assert.Null(blocked.Enrollment);
        Assert.Equal(PaidBlockedError, blocked.Error);
        Assert.Empty(env.Db.Enrollments.Where(e => e.CourseId == 6));
        Assert.Null(env.Students.Find("Asha"));

        env.Enrollments.Enroll(5, "Asha");
        env.CompleteLessons("Asha", 5, 4);
        Assert.NotNull(env.Enrollments.Enroll(6, "Asha").Enrollment);
    }

    [Fact]
    public void Checkout_BundleWithUnfinishedPrerequisite_IsBlocked()
    {
        // Course 5 is free, so it cannot sit in a cart. Use a paid chain instead: course 6 requires paid course 2 (JavaScript, 40).
        using var env = new TestEnv();
        env.Db.CoursePrerequisites.Add(new CoursePrerequisite { CourseId = 6, RequiresCourseId = 2 });
        env.Db.SaveChanges();
        PutInCart(env, "Asha", 2);
        PutInCart(env, "Asha", 6);

        var (order, error) = env.Shop.Checkout("Asha", null);
        Assert.Null(order);
        Assert.Equal("Finish JavaScript for the Web before you enroll in this course.", error);
        Assert.Empty(env.Db.Orders);
        Assert.False(env.Enrollments.IsEnrolled("Asha", 2));
        Assert.False(env.Enrollments.IsEnrolled("Asha", 6));
    }

    [Fact]
    public void Checkout_PrerequisiteFreeCourse_StillCheckoutsNormally()
    {
        using var env = WithPaidPath();

        Assert.NotNull(env.Shop.AddToCart("Asha", 2).Item);
        var (order, error) = env.Shop.Checkout("Asha", null);
        Assert.Null(error);
        Assert.NotNull(order);
        Assert.True(env.Enrollments.IsEnrolled("Asha", 2));
    }
}
