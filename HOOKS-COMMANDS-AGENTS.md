# Hooks, Commands and Agents (trainer reference)

The trainer builds these LIVE in the Day 4 session. This repo has no `.claude/settings.json`, `.claude/commands/` or `.claude/agents/` on purpose.

## What each one is

- **Hook**: a command that Claude Code runs by itself at a set moment (before or after a tool, or when Claude stops). It does not depend on Claude remembering.
- **Slash command**: a saved prompt in a `.md` file. Type `/name` to run it.
- **Agent (subagent)**: a helper Claude with its own instructions, its own tools and its own context. Claude hands it a job and gets a short answer back.

## Examples

| Type | Name | What it does |
| --- | --- | --- |
| Hook | PostToolUse on Edit/MultiEdit/Write | After a `.cs` file changes, run `dotnet build` |
| Hook | PreToolUse on Edit/MultiEdit/Write | Block edits to `.env` and `appsettings.*.json` |
| Hook | Stop | When Claude finishes, run `dotnet test` |
| Command | `/run-app` | Start the backend and frontend and say the URLs |
| Command | `/review-changes` | Review the current git diff |
| Command | `/add-test` | Write a test for a named method |
| Agent | `code-reviewer` | Reads changes, reports bugs and style problems, edits nothing |
| Agent | `test-writer` | Writes xUnit tests and runs them |
| Agent | `ui-designer` | Improves React pages and CSS |

## Hooks

Hooks live in `.claude/settings.json`. Claude Code sends the hook a JSON text on stdin. It holds `tool_input.file_path` for edits.
Exit code 0 means fine. Exit code 2 in a PreToolUse hook blocks the tool call, and the message on stderr goes to Claude.
Windows users: run Claude Code with Git Bash on the PATH so `bash` works.

### 1. Build after a C# edit

Script `.claude/hooks/build-cs.sh` (make it runnable with `chmod +x`):

```bash
#!/usr/bin/env bash
# Runs dotnet build only when the edited file is a .cs file.
input=$(cat)
file=$(echo "$input" | sed -n 's/.*"file_path"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' | head -n 1)
case "$file" in
  *.cs) cd "$CLAUDE_PROJECT_DIR/backend" && dotnet build --nologo -v q 1>&2 || exit 2 ;;
esac
exit 0
```

Settings:

```json
{
  "hooks": {
    "PostToolUse": [
      {
        "matcher": "Edit|MultiEdit|Write",
        "hooks": [
          { "type": "command", "command": "bash \"$CLAUDE_PROJECT_DIR/.claude/hooks/build-cs.sh\"" }
        ]
      }
    ]
  }
}
```

### 2. Block secret files

Script `.claude/hooks/block-secrets.sh`:

```bash
#!/usr/bin/env bash
# Blocks edits to .env and appsettings.*.json files.
input=$(cat)
file=$(echo "$input" | sed -n 's/.*"file_path"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' | head -n 1)
name=$(basename "$file")
case "$name" in
  .env|.env.*|appsettings.*.json)
    echo "Blocked: $name may hold secrets. Ask the user to edit it." 1>&2
    exit 2 ;;
esac
exit 0
```

Settings:

```json
{
  "hooks": {
    "PreToolUse": [
      {
        "matcher": "Edit|MultiEdit|Write",
        "hooks": [
          { "type": "command", "command": "bash \"$CLAUDE_PROJECT_DIR/.claude/hooks/block-secrets.sh\"" }
        ]
      }
    ]
  }
}
```

### 3. Run tests when Claude finishes

```json
{
  "hooks": {
    "Stop": [
      {
        "hooks": [
          { "type": "command", "command": "cd \"$CLAUDE_PROJECT_DIR/backend\" && dotnet test --nologo -v q" }
        ]
      }
    ]
  }
}
```

To use all three, put the three event keys (`PostToolUse`, `PreToolUse`, `Stop`) inside one `"hooks"` object.

## Slash commands

A command is a `.md` file in `.claude/commands/`. The file name is the command name. Custom commands and skills are now very similar: a skill can also be called with `/name`. Use a command for a short saved prompt. Use a skill when you need extra files or Claude should pick it up on its own.

Sample `.claude/commands/review-changes.md`:

```markdown
---
description: Review the current git changes for bugs and style
---

Look at the current git diff. Focus on: $ARGUMENTS

1. List bugs first, then style problems.
2. Check the rules in CLAUDE.md.
3. Do not edit any file. Only report.
```

Run it like this: `/review-changes the quiz scoring`. The words after the name replace `$ARGUMENTS`.

## Agents

Type `/agents` to list, create and edit agents. Claude can write the file for you. Files live in `.claude/agents/`.

Sample `.claude/agents/code-reviewer.md`:

```markdown
---
name: code-reviewer
description: Reviews code changes for bugs, style and missing tests. Use after any code change.
tools: Read, Grep, Glob, Bash
model: sonnet
---

You are a careful code reviewer for LearnHub (.NET 8 API and React).

1. Run `git diff` to see the changes.
2. Check them against the rules in CLAUDE.md.
3. Report bugs first, then style, then missing tests.
4. Keep the answer under 15 lines. Never edit files.
```

## Demo ideas

1. Ask Claude to rename a variable in `InMemoryStore.cs`. Show the build hook run. Then add a deliberate typo to show the hook fail.
2. Ask Claude to "put my API key in appsettings.Development.json". Show the block hook say no.
3. Write `/review-changes` live, then run it after a change.
4. Create `code-reviewer` with `/agents`, then say "use the code-reviewer agent on my last change".
5. Compare: the same review done in the main chat and done by the agent. Point out that the agent keeps the main chat clean.
