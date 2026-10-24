# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

LearnHub is an e-learning platform (courses, quizzes, shop with cart/coupons/orders/refunds, reviews, Q&A, assignments, instructor payouts). Four parts:

- `backend/LearnHub.Api` – .NET 8 Minimal API + EF Core on MySQL (port 5080)
- `backend/LearnHub.Tests` – xUnit tests
- `frontend/` – React + Vite (port 5173, proxies `/api` to 5080)
- `mcp-server/LearnHub.Mcp` – read-only stdio MCP server over the same MySQL database, registered in `.mcp.json`
- `db/` – `schema.sql`, `seed.sql`, and reset scripts

`EXERCISES.md` holds the Day 5 tasks (refund audit, coupon abuse, etc.). Treat "today" as 2026-10-01; the seed data covers the twelve months before it.

## Commands

```bash
bash db/reset.sh              # or ./db/reset.ps1 – drops/recreates `learnhub` DB, prints row counts (needs mysql client)
cd backend/LearnHub.Api && dotnet run --urls http://localhost:5080
cd frontend && npm install && npm run dev
cd backend && dotnet test                                   # all tests
cd backend && dotnet test --filter "FullyQualifiedName~ShopTests"   # one class / use ~Name for a single test
```

Environment variables:
- `LEARNHUB_DB` – full connection string; overrides `appsettings.json` (API) and the host/port/user/password vars (MCP server).
- `LEARNHUB_DB_HOST/PORT/USER/PASSWORD` – used by `db/reset.*` and the MCP server (defaults `127.0.0.1`, `3306`, `root`; the README gives password `LearnHub!23`).
- `LEARNHUB_NOW=2026-10-01T12:00:00Z` – makes the API and MCP server treat that instant as "now". Set it, otherwise deadlines and refund windows are judged against the real date.

## Architecture

**Backend layering:** `Program.cs` registers every service as scoped and calls one `Map*Endpoints()` per area. `Endpoints/*` only map URLs to services; business rules live in `Services/*` (DTOs in `Services/Dtos.cs`, `IClock`/`StudentDirectory` in `Services/Infrastructure.cs`). `Reports/PayoutReportBuilder` builds the monthly payout report. `Data/LearnHubDbContext` and `Models/Entities.cs` map the tables. Time must come from the injected `IClock`, never `DateTime.Now`, so tests and `LEARNHUB_NOW` can control it. Students are identified by name via `StudentDirectory` (no auth).

**Tests:** `TestEnv` builds an in-memory SQLite database from the small fixed set in `TestData` and wires every service to it with a `FakeClock`; no MySQL needed. `CustomWebApplicationFactory` is used for HTTP-level tests (`ApiTests`). New services must be added to `TestEnv` as well as `Program.cs`.

**Frontend:** one file per page in `src/pages`, shared components in `src/components`, and every backend call goes through `src/api.js`.

**MCP server:** `Tools/*` (Catalog, Shop, Student) are auto-discovered via `WithToolsFromAssembly()`. All DB access goes through `Db.QueryAsync`, which uses parameterised queries, a read-only session and a 500-row cap. Logging goes to stderr because stdout carries the protocol. Keep tools read-only. `.mcp.json` launches it with `dotnet run --project mcp-server/LearnHub.Mcp`.

## Hooks

A commit refreshes the "Recent commits" block below and amends it into that commit (`scripts/update-claude-md.js`). Two triggers share the script: the git hook `.githooks/post-commit` (enable once per clone with `git config core.hooksPath .githooks`) and a Claude Code PostToolUse hook in `.claude/settings.json` that fires after Claude runs `git commit`. Set `CLAUDE_MD_HOOK=1` to skip. Do not edit between the AUTO markers.

## Recent commits

<!-- AUTO:RECENT-COMMITS:START -->
- (this commit) Add reviews and prerequisites audit report
- 7523288 Add prerequisite enforcement tests
- 83918db Enforce prerequisites on every enrolment path
- 3785387 Add review enrolment tests
- aab4e58 Require enrolment to review a course
- e3d21f7 Document at-risk student selection method
- d142ca7 Add at-risk student call list
- 6377f64 Add learner completion and drop-off analysis
- 9831b88 Add coupon audit session handoff
- 9e8ea77 Add payout reconciliation report
<!-- AUTO:RECENT-COMMITS:END -->
