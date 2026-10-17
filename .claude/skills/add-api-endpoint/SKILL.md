---
name: add-api-endpoint
description: Adds a new API feature to the LearnHub backend (model, store data, endpoints file, registration, validation). Use when the user asks for a new endpoint, a new backend resource, or a feature that needs new data on the server, such as quizzes, reviews or progress.
---

# Add an API endpoint

Work in `backend/LearnHub.Api`. Copy the style of `Endpoints/EnrollmentEndpoints.cs`.

## Steps

1. Create a model in `Models/Xxx.cs`.
   - Namespace `LearnHub.Api.Models`. Plain class. Give every property a default (`= ""`, `= new()`).
   - See `Models/Enrollment.cs` as an example.
2. Add data to `Data/InMemoryStore.cs`.
   - Add `public List<Xxx> Xxxs { get; } = new();`.
   - If the feature needs starting data, add sample items with unique ids.
3. Create `Endpoints/XxxEndpoints.cs`.
   - `public static class XxxEndpoints` with `public static void MapXxxEndpoints(this WebApplication app)`.
   - Routes start with `/api/`. Take `InMemoryStore store` as a parameter.
   - For a POST body, add a `public record XxxRequest(...)` at the top of the file.
4. Add validation the LearnHub way.
   - Missing or bad input: `Results.BadRequest(new { error = "field is required." })` (400).
   - Parent item not found (for example the course id): `Results.NotFound()` (404) for GET routes, or `BadRequest(new { error = "Course not found." })` for POST bodies.
   - Duplicate: `Results.Conflict(new { error = "..." })` (409).
   - Create: `Results.Created($"/api/xxxs/{item.Id}", item)`.
   - Compare text with `StringComparison.OrdinalIgnoreCase`.
5. Register it in `Program.cs`: add `app.MapXxxEndpoints();` below the other Map lines.
6. Build: `cd backend && dotnet build`. Fix any errors until there are 0.
7. Optional check: run the API and test with `curl localhost:5080/api/xxxs`. Stop the API after.
8. Tell the user which files you added or changed. Do not touch the frontend unless asked.
