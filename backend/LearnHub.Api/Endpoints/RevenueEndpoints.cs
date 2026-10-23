using LearnHub.Api.Data;

namespace LearnHub.Api.Endpoints;

public static class RevenueEndpoints
{
    public static void MapRevenueEndpoints(this WebApplication app)
    {
        app.MapGet("/api/instructors/revenue", (InMemoryStore store) =>
            store.GetInstructorRevenue().Select(r => new { r.Instructor, r.GrossSales, r.Refunds, r.InstructorShare }));
    }
}
