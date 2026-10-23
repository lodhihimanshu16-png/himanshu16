---
name: api-endpoint-builder
description: Build a new REST API endpoint for the LearnHub .NET 8 minimal API (model, in-memory store, endpoint file, registration and optional frontend client function). Use when the user asks to add, create or build an API endpoint, route, or REST resource in the backend.
---

# API Endpoint Builder (LearnHub)

Add endpoints that match the existing code in `backend/LearnHub.Api`. Read the code before you write anything.

## Steps

1. **Confirm the details.** Work out the resource name, the HTTP verbs, the route, and the request and response shape. Ask only for what is missing.
2. **Read the conventions.** Open `Endpoints/CourseEndpoints.cs`, `Endpoints/EnrollmentEndpoints.cs`, `Program.cs`, `Data/InMemoryStore.cs` and the matching file in `Models/`.
3. **Model.** Add or extend a class in `Models/` (plain class with `{ get; set; }` properties, as in `Enrollment.cs`). Request bodies that need a record go in the endpoint file, as `EnrollRequest` does.
4. **Store.** Put the data in `InMemoryStore` only. Add a list property, and seed data if the resource needs it. Use the same id pattern as the existing code (`Count + 1`, or a fixed seed id).
5. **Endpoint file.** Create `Endpoints/<Name>Endpoints.cs` (see the template below).
6. **Register.** Add `app.Map<Name>Endpoints();` to `Program.cs`, after the existing calls.
7. **Frontend (only if asked).** Add a function to `frontend/src/api.js` that uses the existing `request` helper. Do not add a second fetch wrapper.
8. **Verify.** Run `dotnet build` in `backend/LearnHub.Api`. Fix any errors before you finish. If the API is running, check the route with `curl`.
9. **Report.** List the routes added and the files changed.

## Conventions

- Route prefix is `/api/<resource-plural>`. Use typed route constraints, such as `{id:int}`.
- Inject `InMemoryStore store` as a parameter. Do not use `new InMemoryStore()`.
- Return anonymous projections or DTOs, not the stored entity, when the entity has data the client should not see. The existing course list is an example.
- Responses:
  - `Results.Ok(...)` for reads and updates.
  - `Results.Created($"/api/<resource>/{id}", entity)` for creates.
  - `Results.NoContent()` for deletes.
  - `Results.NotFound()` when an item does not exist.
  - `Results.BadRequest(new { error = "..." })` for invalid input.
  - `Results.Conflict(new { error = "..." })` for duplicates.
- Error bodies always use the shape `{ error = "..." }`, which the frontend reads.
- Trim and validate string input before use, as the enrolment endpoint does.
- Keep the code in the existing style: expression-bodied lambdas, no controllers, no extra services unless the user asks for them.

## Template

```csharp
using LearnHub.Api.Data;
using LearnHub.Api.Models;

namespace LearnHub.Api.Endpoints;

public static class ReviewEndpoints
{
    public static void MapReviewEndpoints(this WebApplication app)
    {
        app.MapGet("/api/reviews", (InMemoryStore store) =>
            store.Reviews.Select(r => new { r.Id, r.CourseId, r.Rating, r.Comment }));

        app.MapGet("/api/reviews/{id:int}", (int id, InMemoryStore store) =>
        {
            var review = store.Reviews.FirstOrDefault(r => r.Id == id);
            return review is null ? Results.NotFound() : Results.Ok(review);
        });

        app.MapPost("/api/reviews", (Review review, InMemoryStore store) =>
        {
            if (review.Rating is < 1 or > 5)
                return Results.BadRequest(new { error = "rating must be between 1 and 5." });

            review.Id = store.Reviews.Count + 1;
            store.Reviews.Add(review);

            return Results.Created($"/api/reviews/{review.Id}", review);
        });

        app.MapPut("/api/reviews/{id:int}", (int id, Review update, InMemoryStore store) =>
        {
            var review = store.Reviews.FirstOrDefault(r => r.Id == id);
            if (review is null) return Results.NotFound();

            review.Rating = update.Rating;
            review.Comment = update.Comment;

            return Results.Ok(review);
        });

        app.MapDelete("/api/reviews/{id:int}", (int id, InMemoryStore store) =>
        {
            var review = store.Reviews.FirstOrDefault(r => r.Id == id);
            if (review is null) return Results.NotFound();

            store.Reviews.Remove(review);
            return Results.NoContent();
        });
    }
}
```

Use the template as a shape only. Rename the types, fields, validation and status codes to fit the request.
