using LearnHub.Api.Data;

namespace LearnHub.Tests;

public class InMemoryStoreTests
{
    [Fact]
    public void Courses_HaveUniqueIds()
    {
        var store = new InMemoryStore();
        var ids = store.Courses.Select(c => c.Id).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Theory]
    [InlineData("data")]
    [InlineData("DATA")]
    public void Search_IsCaseInsensitive(string search)
    {
        var store = new InMemoryStore();
        Assert.Contains(store.SearchCourses(search), c => c.Title == "Data Analysis with SQL");
    }

    [Fact]
    public void Search_WithNoMatch_ReturnsNothing()
    {
        var store = new InMemoryStore();
        Assert.Empty(store.SearchCourses("zzz-no-match"));
    }

    [Fact]
    public void Enroll_Works_ThenRejectsDuplicateIgnoringCase()
    {
        var store = new InMemoryStore();
        var first = store.Enroll(1, "Asha");
        var second = store.Enroll(1, "ASHA");

        Assert.NotNull(first.Enrollment);
        Assert.Null(second.Enrollment);
        Assert.Equal("Already enrolled in this course.", second.Error);
    }

    [Fact]
    public void Enroll_RejectsEmptyNameAndUnknownCourse()
    {
        var store = new InMemoryStore();
        Assert.Equal("studentName is required.", store.Enroll(1, "  ").Error);
        Assert.Equal("Course not found.", store.Enroll(999, "Asha").Error);
    }

    [Fact]
    public void Quiz_HasTwoQuestionsForEachLessonOfFirstThreeCourses()
    {
        var store = new InMemoryStore();
        foreach (var lesson in store.Courses.Take(3).SelectMany(c => c.Lessons))
            Assert.Equal(2, store.GetQuiz(lesson.Id).Count);
    }

    [Fact]
    public void Quiz_AllCorrect_ScoresFullAndPasses()
    {
        var store = new InMemoryStore();
        var answers = store.GetQuiz(1).Select(q => (q.Id, q.CorrectIndex));

        var result = store.ScoreQuiz(1, answers);

        Assert.Equal((2, 2, true), result);
    }

    [Fact]
    public void Quiz_HalfCorrect_Passes_NoneCorrect_Fails()
    {
        var store = new InMemoryStore();
        var qs = store.GetQuiz(1);
        var wrong = (qs[1].CorrectIndex + 1) % 4;

        var half = store.ScoreQuiz(1, new[] { (qs[0].Id, qs[0].CorrectIndex), (qs[1].Id, wrong) });
        var none = store.ScoreQuiz(1, new[] { (qs[0].Id, (qs[0].CorrectIndex + 1) % 4), (qs[1].Id, wrong) });

        Assert.Equal((1, 2, true), half);
        Assert.Equal((0, 2, false), none);
    }

    [Fact]
    public void Progress_RequiresEnrollment()
    {
        var store = new InMemoryStore();
        Assert.Equal("Student is not enrolled in this course.", store.MarkLessonComplete("Asha", 1, 1));
    }

    [Fact]
    public void Progress_IsIdempotent()
    {
        var store = new InMemoryStore();
        store.Enroll(1, "Asha");

        Assert.Null(store.MarkLessonComplete("Asha", 1, 1));
        Assert.Null(store.MarkLessonComplete("asha", 1, 1));

        Assert.Single(store.Progress);
    }

    [Fact]
    public void Progress_Percent_IsCompletedOverTotal()
    {
        var store = new InMemoryStore();
        store.Enroll(1, "Asha"); // course 1 has 4 lessons
        store.MarkLessonComplete("Asha", 1, 1);

        var p = Assert.Single(store.GetProgress("Asha"));

        Assert.Equal(1, p.CompletedLessons);
        Assert.Equal(4, p.TotalLessons);
        Assert.Equal(25, p.Percent);
    }

    [Fact]
    public void Progress_RejectsLessonFromAnotherCourse()
    {
        var store = new InMemoryStore();
        store.Enroll(1, "Asha");
        Assert.Equal("Lesson not found in this course.", store.MarkLessonComplete("Asha", 1, 5));
    }
}
