# Skill ideas (for the trainer)

| Skill | What it does | Demo prompt to type | Feature it builds |
| --- | --- | --- | --- |
| `add-api-endpoint` | Adds model, store data, endpoints and validation to the backend | "Add a quiz for each lesson" | Quizzes per lesson |
| `add-react-page` | Adds a page, route, nav link and api helper to the frontend | "Add a Progress page that shows lessons done per course" | Progress page |
| `write-api-tests` | Creates the xUnit test project and runs `dotnet test` | "Write tests for the backend" | Tests |
| `seed-sample-data` | Adds realistic courses, lessons and quiz questions to the store | "Add 4 more courses with lessons" | More sample data |

## How a skill works

1. A skill is a folder in `.claude/skills/` with a `SKILL.md` file inside.
2. The `description` in the top of `SKILL.md` is how Claude decides to use the skill.
3. Claude loads the full steps only when your request matches the description.
4. You can also call a skill yourself by typing `/skill-name`.
5. A skill folder can hold extra files, such as `templates/ExampleTest.cs`.
