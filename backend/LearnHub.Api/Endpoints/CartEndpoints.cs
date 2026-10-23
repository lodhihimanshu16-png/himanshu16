using LearnHub.Api.Data;

namespace LearnHub.Api.Endpoints;

public record CartAddRequest(string? StudentName, int CourseId);

public static class CartEndpoints
{
    public static void MapCartEndpoints(this WebApplication app)
    {
        app.MapGet("/api/cart", (string? studentName, InMemoryStore store) =>
            store.GetCart(studentName).Select(x => new
            {
                CourseId = x.Course.Id, x.Course.Title, x.Course.Price, x.Course.Category, x.Course.Instructor
            }));

        app.MapPost("/api/cart", (CartAddRequest request, InMemoryStore store) =>
        {
            var (item, kind, error) = store.AddToCartChecked(request.StudentName, request.CourseId);
            if (item is null)
                return kind == "Conflict"
                    ? Results.Conflict(new { error })
                    : Results.BadRequest(new { error });
            return Results.Created("/api/cart", new { item.StudentName, item.CourseId, item.AddedAt });
        });

        app.MapDelete("/api/cart", (string? studentName, int courseId, InMemoryStore store) =>
            store.RemoveFromCart(studentName, courseId) ? Results.NoContent() : Results.NotFound());
    }
}
