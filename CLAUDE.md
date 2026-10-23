# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

Run all commands from the folder shown. The backend and frontend must both be running for the app to work: Vite proxies `/api` to the backend.

**Backend** (from `backend/LearnHub.Api`, .NET 8)
- `dotnet run`: serves on http://localhost:5080 (set in `Properties/launchSettings.json`)
- `dotnet build`: compile check. Build output in `bin/` and `obj/` is gitignored.

**Frontend** (from `frontend`, Node 20+)
- `npm install`
- `npm run dev`: Vite dev server on http://localhost:5173
- `npm run build`: production build to `dist/`
- `npm run preview`: serve the built `dist/`

There is no test project and no lint script, so there is no single-test command. Verify changes with `dotnet build` and `npm run build`, then check the API with `curl http://localhost:5080/api/courses` or in the running app.

## Architecture

LearnHub is a small e-learning app: a .NET 8 minimal API serving JSON, and a React single-page app that calls it. Both sides keep all state in memory or in the browser; there is no database.

**Backend** (`backend/LearnHub.Api`)
- `Program.cs` wires everything. `InMemoryStore` is a singleton, and each endpoint lambda receives it as a parameter. Each area is mapped by a `Map<Area>Endpoints(this WebApplication)` extension in `Endpoints/`. A new area must be registered in `Program.cs`.
- `Data/InMemoryStore.cs` is the only data store. It holds seeded `Courses` (lessons nested inside each course) and the `Enrollments` list. All enrollments are lost when the process stops.
- IDs are assigned as `Count + 1`. This is safe only because nothing deletes records. Lesson IDs are unique across all courses (1 to 24), and a lesson has no `CourseId`, so it can only be reached through its course.
- Responses are anonymous projections, not the stored models. The course list omits lessons and adds `LessonCount`. The course detail includes lessons. The enrollment list joins in `CourseTitle` from the store.
- Enrollment rules live in `Endpoints/EnrollmentEndpoints.cs`. The student name is trimmed and matched case-insensitively. Unknown course gives 400, duplicate gives 409, and a missing name gives 400. Errors use the body shape `{ error = "..." }`.
- `GET /api/enrollments` with no `studentName` returns an empty list, not an error.
- C# PascalCase properties reach the frontend as camelCase JSON (`lessonCount`, `courseTitle`). If you add a property in C#, the frontend sees it under that camelCase name.
- CORS allows only `http://localhost:5173`. In development the Vite proxy means the browser never makes a cross-origin call.

**Frontend** (`frontend/src`, React 19, react-router-dom v6, plain JavaScript, no TypeScript)
- `App.jsx` defines three routes: `/` (courses), `/courses/:id` (detail and enroll), and `/my-courses`. Pages are in `pages/`, shared pieces in `components/`.
- `api.js` is the only file that calls `fetch`. Its `request()` helper parses JSON and throws an `Error` using `body.error`. Add new backend calls there, through `request()`.
- There is no global state library. Each page loads its own data in a `useEffect`.
- There are no accounts. The student's name is the identity. It is saved in `localStorage` under `studentName` and sent with each enrollment. "My Courses" lists enrollments for that name.
- Styling is one file, `index.css`, with CSS variables in `:root` and plain class names. Reuse existing classes (`panel`, `row`, `message ok|error`, `grid`, `card`) before adding new ones.

## Adding an endpoint

The project skill `.claude/skills/api-endpoint-builder/SKILL.md` describes the steps and conventions for a new REST resource. Follow it rather than rebuilding the pattern from scratch.
