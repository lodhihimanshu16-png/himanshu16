# LearnHub (Day 2)

LearnHub is a small e-learning website. You can browse courses, read lessons, and enroll.
It has a backend (C#, .NET 8) and a frontend (React). You run both at the same time.

## What you need installed

1. .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
2. Node.js 20 or newer: https://nodejs.org
3. VS Code: https://code.visualstudio.com
4. Claude Code extension for VS Code: https://docs.claude.com/en/docs/claude-code/ide-integrations

## How to run it

You need two terminals. Keep both open.

**Terminal 1 (backend)**

1. `cd backend/LearnHub.Api`
2. `dotnet run`

Wait until you see `Now listening on: http://localhost:5080`.

**Terminal 2 (frontend)**

1. `cd frontend`
2. `npm install`
3. `npm run dev`

Now open http://localhost:5173 in your browser.

## Check the backend alone

Open http://localhost:5080/api/courses in your browser. You should see a list of courses as JSON.

## Folder map

| Folder | What it is for |
| --- | --- |
| `backend/LearnHub.Api/Models` | The shapes of our data: Course, Lesson, Enrollment |
| `backend/LearnHub.Api/Data` | The in-memory store with the sample courses |
| `backend/LearnHub.Api/Endpoints` | The API routes, one file per area |
| `frontend/src/pages` | The three screens of the website |
| `frontend/src/components` | Small shared pieces (top bar, course card) |
| `frontend/src/api.js` | All calls from the website to the backend |

Data is kept in memory. When you stop the backend, enrollments are gone.

## If something goes wrong

1. **"Address already in use" (port busy).** Another program uses port 5080 or 5173. Close it, or stop the old `dotnet run` / `npm run dev` and start again.
2. **Wrong .NET version.** Run `dotnet --version`. It should start with `8.`. If not, install the .NET 8 SDK from the link above.
3. **`npm install` fails.** Run `node --version`. It should be 20 or higher. Then delete the `frontend/node_modules` folder and run `npm install` again.
