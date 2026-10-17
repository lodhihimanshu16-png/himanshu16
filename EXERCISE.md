# Day 4 Exercise: Hooks, Slash Commands and Agents

A hook is a small command that Claude Code runs for you at a set moment, such as after a file edit.
A slash command is a saved prompt you run by typing its name, like `/run-checks`.
An agent (subagent) is a helper Claude with its own job and its own memory of the task.

## Steps

0. Make a commit before you start (`git add . && git commit -m start`) so Claude can see what changed with `git diff`.
1. Run the app and the tests. Follow README.md. Run `dotnet test` from `backend/`. Open the Quiz and My Progress pages.
2. Hook. Add a hook so `dotnet build` runs after Claude edits a C# file.
   Create the script file first, then add the hook. Type `/hooks` or edit `.claude/settings.json`.
   HOOKS-COMMANDS-AGENTS.md has examples to look at. Try writing your own first.
   Ask Claude to make a small change. Watch the hook run.
3. Slash command. Create `.claude/commands/run-checks.md`.
   It should build the backend, run the tests and build the frontend.
   Run it with `/run-checks`.
4. Agent. Type `/agents` and create a `code-reviewer` agent.
   Ask Claude: "Use the code-reviewer agent to review my last change."
5. Use all three to build "Bookmarks".
   - A student can bookmark a course with a bookmark icon on the course card.
   - A "My Bookmarks" page lists the bookmarked courses.
   - Let the hook build, review with your agent, check with your command.
6. Write 3 lines: what each one (hook, command, agent) is good for.

## What to bring to Day 5

Your `.claude` folder and the bookmarks feature.
