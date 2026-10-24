namespace LearnHub.Tests;

public class ReviewTests
{
    private static TestEnv EnrolledEnv(params string[] names)
    {
        var env = new TestEnv();
        foreach (var n in names) env.Enrollments.Enroll(1, n);
        return env;
    }

    [Fact]
    public void Review_IsSaved_AndListedNewestFirst()
    {
        using var env = EnrolledEnv("Asha", "Ben");
        Assert.NotNull(env.Reviews.AddReview(1, "Asha", 5, "Loved it").Review);
        env.Clock.Now = TestData.Today.AddDays(1);
        Assert.NotNull(env.Reviews.AddReview(1, "Ben", 3, "Fine").Review);

        var list = env.Reviews.GetReviews(1);
        Assert.Equal(new[] { "Ben", "Asha" }, list.Select(r => r.StudentName));
        Assert.Equal("Loved it", list[1].Comment);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Review_StarsMustBeOneToFive(int stars)
    {
        using var env = EnrolledEnv("Asha");
        Assert.Equal("stars must be between 1 and 5.", env.Reviews.AddReview(1, "Asha", stars, "x").Error);
    }

    [Fact]
    public void Review_RequiresNameAndKnownCourse()
    {
        using var env = EnrolledEnv("Asha");
        Assert.Equal("studentName is required.", env.Reviews.AddReview(1, " ", 4, "x").Error);
        Assert.Equal("Course not found.", env.Reviews.AddReview(99, "Asha", 4, "x").Error);
    }

    [Fact]
    public void Review_OnePerStudentPerCourse_IgnoringCase()
    {
        using var env = EnrolledEnv("Asha");
        env.Reviews.AddReview(1, "Asha", 4, "Good");
        Assert.Equal("You have already reviewed this course.", env.Reviews.AddReview(1, "ASHA", 2, "Again").Error);
    }

    [Fact]
    public void Course_ShowsAverageRatingAndCount()
    {
        using var env = EnrolledEnv("Asha", "Ben");
        env.Reviews.AddReview(1, "Asha", 5, "");
        env.Reviews.AddReview(1, "Ben", 4, "");

        var detail = env.Courses.Get(1)!;
        Assert.Equal(4.5, detail.AverageRating);
        Assert.Equal(2, detail.ReviewCount);
        Assert.Equal(0, env.Courses.Search("Cloud").First().ReviewCount);
        Assert.Equal(4.5, env.Courses.Search("C#").Single().AverageRating);
    }

    private const string NotEnrolledError = "You must be enrolled in this course to review it.";

    [Fact]
    public void Review_FromEnrolledStudent_IsAccepted()
    {
        using var env = EnrolledEnv("Asha");
        var result = env.Reviews.AddReview(1, "Asha", 4, "Solid");
        Assert.Null(result.Error);
        Assert.Equal(4, result.Review!.Stars);
        Assert.Single(env.Reviews.GetReviews(1));
    }

    [Fact]
    public void Review_FromNonEnrolledStudent_IsRejected()
    {
        using var env = new TestEnv();
        env.Students.GetOrCreate("Asha"); // exists, but has no enrolment in course 1
        var reviewsBefore = env.Db.Reviews.Count();

        var result = env.Reviews.AddReview(1, "Asha", 4, "Nice");

        Assert.Null(result.Review);
        Assert.Equal(NotEnrolledError, result.Error);
        Assert.Equal(reviewsBefore, env.Db.Reviews.Count());
    }

    [Fact]
    public void Review_FromUnknownStudent_IsRejected_AndNoStudentIsCreated()
    {
        using var env = EnrolledEnv("Asha");
        var studentsBefore = env.Db.Students.Count();
        var reviewsBefore = env.Db.Reviews.Count();

        var result = env.Reviews.AddReview(1, "Never Seen Before", 4, "Hello");

        Assert.Null(result.Review);
        Assert.Equal(NotEnrolledError, result.Error);
        Assert.Equal(studentsBefore, env.Db.Students.Count());
        Assert.Equal(reviewsBefore, env.Db.Reviews.Count());
        Assert.Null(env.Students.Find("Never Seen Before"));
    }

    [Fact]
    public void Review_FiveStarBurstFromNewNames_AllRejected_AndCourseCountUnchanged()
    {
        using var env = EnrolledEnv("Asha");
        var reviewCountBefore = env.Courses.Get(1)!.ReviewCount;
        var studentsBefore = env.Db.Students.Count();

        for (var i = 1; i <= 5; i++)
        {
            var result = env.Reviews.AddReview(1, $"Burst Bot {i}", 5, "Five stars");
            Assert.Null(result.Review);
            Assert.Equal(NotEnrolledError, result.Error);
        }

        Assert.Equal(reviewCountBefore, env.Courses.Get(1)!.ReviewCount);
        Assert.Empty(env.Reviews.GetReviews(1).Where(r => r.StudentName.StartsWith("Burst Bot")));
        Assert.Equal(studentsBefore, env.Db.Students.Count());
    }

    [Fact]
    public void Review_FromStudentEnrolledInDifferentCourse_IsRejected()
    {
        using var env = new TestEnv();
        env.Enrollments.Enroll(2, "Asha"); // enrolled in course 2 only

        var result = env.Reviews.AddReview(1, "Asha", 5, "Wrong course");

        Assert.Null(result.Review);
        Assert.Equal(NotEnrolledError, result.Error);
        Assert.Empty(env.Reviews.GetReviews(1));
    }
}
