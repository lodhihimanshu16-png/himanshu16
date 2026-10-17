---
name: write-api-tests
description: Creates and runs xUnit tests for the LearnHub backend (InMemoryStore and endpoint logic). Use when the user asks for tests, test coverage, or to check that the backend works.
---

# Write API tests

## Steps

1. Check whether `backend/LearnHub.Tests` exists. If it does, go to step 3.
2. Create the test project.
   1. `cd backend`
   2. `dotnet new xunit -n LearnHub.Tests -f net8.0`
   3. `dotnet sln LearnHub.sln add LearnHub.Tests/LearnHub.Tests.csproj`
   4. `dotnet add LearnHub.Tests reference LearnHub.Api/LearnHub.Api.csproj`
   5. For endpoint tests add: `dotnet add LearnHub.Tests package Microsoft.AspNetCore.Mvc.Testing --version 8.0.*`
      (Ask the user first. CLAUDE.md says no new packages without asking.)
   6. Delete the default `UnitTest1.cs`.
3. Read `templates/ExampleTest.cs` in this skill folder. Copy its style.
4. Write tests in `backend/LearnHub.Tests`, one file per area, for example `InMemoryStoreTests.cs`.
   - Store tests: `new InMemoryStore()`, then check `Courses`, ids are unique, every course has lessons.
   - Endpoint tests: use `WebApplicationFactory<Program>`. For this, add `public partial class Program { }` at the bottom of `LearnHub.Api/Program.cs`.
   - Cover the happy path and the 400, 404 and 409 cases.
5. Run: `cd backend && dotnet test`. Fix failures. Do not delete a test just to make it pass.
6. Report how many tests passed.
