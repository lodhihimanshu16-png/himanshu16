using LearnHub.Api.Data;
using LearnHub.Api.Models;

namespace LearnHub.Api.Endpoints;

public record EnrollRequest(int CourseId, string? StudentName);

public static class EnrollmentEndpoints
{
    public static void MapEnrollmentEndpoints(this WebApplication app)
    {
        app.MapPost("/api/enrollments", (EnrollRequest request, InMemoryStore store) =>
        {
            var name = request.StudentName?.Trim();
            if (string.IsNullOrEmpty(name))
                return Results.BadRequest(new { error = "studentName is required." });

            if (!store.Courses.Any(c => c.Id == request.CourseId))
                return Results.BadRequest(new { error = "Course not found." });

            var alreadyEnrolled = store.Enrollments.Any(e =>
                e.CourseId == request.CourseId &&
                string.Equals(e.StudentName, name, StringComparison.OrdinalIgnoreCase));
            if (alreadyEnrolled)
                return Results.Conflict(new { error = "Already enrolled in this course." });

            var enrollment = new Enrollment
            {
                Id = store.Enrollments.Count + 1,
                CourseId = request.CourseId,
                StudentName = name
            };
            store.Enrollments.Add(enrollment);

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
