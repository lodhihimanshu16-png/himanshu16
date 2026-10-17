using LearnHub.Api.Data;
using Xunit;

namespace LearnHub.Tests;

public class InMemoryStoreTests
{
    [Fact]
    public void Store_HasSampleCourses()
    {
        var store = new InMemoryStore();

        Assert.NotEmpty(store.Courses);
    }

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

        var found = store.Courses
            .Where(c => c.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.Contains(found, c => c.Title == "Data Analysis with SQL");
    }
}
