using LearnHub.Api.Data;

namespace LearnHub.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapGet("/api/orders", (string? studentName, InMemoryStore store) =>
            store.GetOrders(studentName).Select(CheckoutEndpoints.OrderShape));

        app.MapPost("/api/orders/{id:int}/refund", (int id, InMemoryStore store) =>
        {
            var (order, kind, error) = store.RefundOrder(id);
            if (order is not null) return Results.Ok(CheckoutEndpoints.OrderShape(order));
            return kind switch
            {
                "notfound" => Results.NotFound(new { error }),
                "conflict" => Results.Conflict(new { error }),
                _ => Results.BadRequest(new { error })
            };
        });
    }
}
