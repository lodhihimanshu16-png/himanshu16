# LearnHub (Day 4)

LearnHub is a small e-learning website. You can browse courses, read lessons, take quizzes, track progress, and enroll.
It has a backend (C#, .NET 8) and a frontend (React). You run both at the same time.

## Day 4 hands-on lab

Start with `FEATURE.md`, then `HANDS-ON.md`. FEATURE.md says what we build (paid courses). HANDS-ON.md says how we build it with Claude, step by step.
The take-home exercise is still `EXERCISE.md` (Bookmarks).

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

## Run the tests

1. `cd backend`
2. `dotnet test`

All tests should pass. They check enrollment, search, quiz scoring and progress.

## Check the backend alone

Open http://localhost:5080/api/courses in your browser. You should see a list of courses as JSON.

## Folder map

| Folder | What it is for |
| --- | --- |
| `backend/LearnHub.Api/Models` | The shapes of our data: Course, Lesson, Enrollment, QuizQuestion, LessonProgress |
| `backend/LearnHub.Api/Data` | The in-memory store with the sample courses |
| `backend/LearnHub.Tests` | The tests (run with `dotnet test` from `backend/`) |
| `backend/LearnHub.Api/Endpoints` | The API routes, one file per area |
| `frontend/src/pages` | The screens of the website (courses, quiz, progress) |
| `frontend/src/components` | Small shared pieces (top bar, course card) |
| `frontend/src/api.js` | All calls from the website to the backend |
| `.claude/skills` | Skills: step-by-step guides that Claude Code follows for common jobs |
| `FEATURE.md` | The spec for the Day 4 lab (what we build) |
| `HANDS-ON.md` | The Day 4 lab script (how we build it with Claude) |
| `hands-on` | Trainer notes and backup answers for the lab |
| `HOOKS-COMMANDS-AGENTS.md` | Reference: examples of hooks, commands and agents |

Data is kept in memory. When you stop the backend, enrollments are gone.

## If something goes wrong

1. **"Address already in use" (port busy).** Another program uses port 5080 or 5173. Close it, or stop the old `dotnet run` / `npm run dev` and start again.
2. **Wrong .NET version.** Run `dotnet --version`. It should start with `8.`. If not, install the .NET 8 SDK from the link above.
3. **`npm install` fails.** Run `node --version`. It should be 20 or higher. Then delete the `frontend/node_modules` folder and run `npm install` again.

Windows: hooks use bash scripts. Run Claude Code from Git Bash or WSL.
