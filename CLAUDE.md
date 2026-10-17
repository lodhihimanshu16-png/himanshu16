# LearnHub

LearnHub is a small e-learning app. Students browse courses, read lessons and enroll.
Data lives in memory only. It resets when the backend stops.

## How to run

1. Backend: `cd backend/LearnHub.Api && dotnet run` (http://localhost:5080)
2. Frontend: `cd frontend && npm install && npm run dev` (http://localhost:5173)
3. Check backend build: `cd backend && dotnet build`
4. Run backend tests: `cd backend && dotnet test`
5. Check frontend build: `cd frontend && npm run build`

## Folder map

- `backend/LearnHub.Api/Models` - data classes (Course, Lesson, Enrollment, QuizQuestion, LessonProgress)
- `backend/LearnHub.Api/Data/InMemoryStore.cs` - all data and the business logic (enroll, quiz scoring, progress), one singleton
- `backend/LearnHub.Api/Endpoints` - one static class per area
- `backend/LearnHub.Api/Program.cs` - registers the store and endpoints
- `backend/LearnHub.Tests` - xUnit tests for the store logic (`cd backend && dotnet test`)
- `frontend/src/pages` - one file per screen
- `frontend/src/components` - small shared pieces
- `frontend/src/api.js` - every backend call
- `frontend/src/index.css` - all styles

## Coding rules

### Backend (.NET 8 Minimal API)
1. Endpoints live in `Endpoints/XxxEndpoints.cs` as `public static class XxxEndpoints`.
2. Each class has `MapXxxEndpoints(this WebApplication app)`. Register it in `Program.cs`.
3. Routes start with `/api/`. Take `InMemoryStore store` as a handler parameter.
4. Return anonymous objects, not the raw model, when the shape differs.
5. Errors: `Results.BadRequest(new { error = "..." })` for bad input,
   `Results.NotFound()` for a missing item, `Results.Conflict(new { error = "..." })` for duplicates.
6. Compare names with `StringComparison.OrdinalIgnoreCase`.
7. Put logic in `InMemoryStore` methods, not in endpoint lambdas, so tests can reach it.

### Store
1. Models are plain classes in `Models/` with default values (`= ""`, `= new()`).
2. All data goes in `InMemoryStore` as a `List<T>`. No database, no other static data.
3. Ids are unique ints. Never reuse an id.

### Frontend (React + Vite, plain JS)
1. One page per file in `src/pages`, default export a function component.
2. Routes are in `App.jsx`. Nav links are in `components/NavBar.jsx`.
3. Every backend call is a function in `src/api.js` that uses `request()`.
4. Pages show errors with `<p className="message error">`.
5. Reuse existing CSS classes (`panel`, `grid`, `row`, `muted`, `tag`) before adding new ones.

### General
1. Do not add new libraries or packages without asking first.
2. Keep code small and simple. Match the style of nearby files.
3. After backend changes run `dotnet build` and `dotnet test`. After frontend changes run `npm run build`.
