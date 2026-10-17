using LearnHub.Api.Data;

namespace LearnHub.Api.Endpoints;

public record ProgressRequest(string? StudentName, int CourseId, int LessonId);

public static class ProgressEndpoints
{
    public static void MapProgressEndpoints(this WebApplication app)
    {
        app.MapPost("/api/progress", (ProgressRequest request, InMemoryStore store) =>
        {
            var error = store.MarkLessonComplete(request.StudentName, request.CourseId, request.LessonId);
            return error is null ? Results.Ok(new { completed = true }) : Results.BadRequest(new { error });
        });

        app.MapGet("/api/progress", (string? studentName, InMemoryStore store) =>
            store.GetProgress(studentName).Select(p => new
            {
                p.CourseId,
                p.CourseTitle,
                CompletedLessons = p.CompletedLessons,
                TotalLessons = p.TotalLessons,
                p.Percent,
                CompletedLessonIds = p.CompletedLessonIds
            }));
    }
}
