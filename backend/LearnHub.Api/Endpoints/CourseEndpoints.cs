using LearnHub.Api.Data;

namespace LearnHub.Api.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this WebApplication app)
    {
        app.MapGet("/api/courses", (string? search, InMemoryStore store) =>
            store.SearchCourses(search)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.ShortDescription,
                c.Category,
                c.Instructor,
                c.Level,
                c.Price,
                LessonCount = c.Lessons.Count
            }));

        app.MapGet("/api/courses/{id:int}", (int id, InMemoryStore store) =>
        {
            var course = store.Courses.FirstOrDefault(c => c.Id == id);
            if (course is null) return Results.NotFound();

            return Results.Ok(new
            {
                course.Id,
                course.Title,
                course.ShortDescription,
                course.Category,
                course.Instructor,
                course.Level,
                course.Price,
                Lessons = course.Lessons.Select(l => new
                {
                    l.Id,
                    l.Title,
                    l.DurationMinutes,
                    l.Content
                })
            });
        });
    }
}
