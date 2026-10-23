using LearnHub.Api.Data;
using LearnHub.Api.Models;

namespace LearnHub.Api.Endpoints;

public record CheckoutRequest(string? StudentName, string? CouponCode);

public static class CheckoutEndpoints
{
    public static object OrderShape(Order o) => new
    {
        o.Id, o.StudentName, o.CreatedAt,
        Lines = o.Lines.Select(l => new { l.CourseId, l.Title, l.Instructor, l.Price, l.BundleDiscount, l.CouponDiscount, l.Final }),
        o.CouponCode, o.Subtotal, o.BundleDiscount, o.CouponDiscount, o.Total, o.Status
    };

    public static void MapCheckoutEndpoints(this WebApplication app)
    {
        app.MapPost("/api/checkout/preview", (CheckoutRequest request, InMemoryStore store) =>
        {
            var q = store.Quote(request.StudentName, request.CouponCode);
            return Results.Ok(new
            {
                Lines = q.Lines.Select(l => new { l.CourseId, l.Title, l.Price, l.BundleDiscount, l.CouponDiscount, l.Final }),
                q.Subtotal, q.BundleDiscount, q.CouponDiscount, q.Total, q.CouponMessage
            });
        });

        app.MapPost("/api/checkout", (CheckoutRequest request, InMemoryStore store) =>
        {
            var (order, error) = store.Checkout(request.StudentName, request.CouponCode);
            return order is null
                ? Results.BadRequest(new { error })
                : Results.Created($"/api/orders/{order.Id}", OrderShape(order));
        });
    }
}
