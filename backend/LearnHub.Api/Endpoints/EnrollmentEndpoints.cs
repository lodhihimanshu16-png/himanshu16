using LearnHub.Api.Data;

namespace LearnHub.Api.Endpoints;

public record EnrollRequest(int CourseId, string? StudentName);

public static class EnrollmentEndpoints
{
    public static void MapEnrollmentEndpoints(this WebApplication app)
    {
        app.MapPost("/api/enrollments", (EnrollRequest request, InMemoryStore store) =>
        {
            var (enrollment, error) = store.Enroll(request.CourseId, request.StudentName);
            if (enrollment is null)
                return error == "Already enrolled in this course."
                    ? Results.Conflict(new { error })
                    : Results.BadRequest(new { error });

            return Results.Created($"/api/enrollments/{enrollment.Id}", enrollment);
        });

        app.MapGet("/api/enrollments", (string? studentName, InMemoryStore store) =>
        {
            var name = studentName?.Trim() ?? "";

            return store.Enrollments
                .Where(e => string.Equals(e.StudentName, name, StringComparison.OrdinalIgnoreCase))
                .Select(e => new
                {
                    e.Id,
                    e.CourseId,
                    CourseTitle = store.Courses.First(c => c.Id == e.CourseId).Title,
                    e.StudentName,
                    e.EnrolledAt
                });
        });
    }
}
