---
description: Review this session, extract learnings, compare with existing skills/agents/hooks/commands, and suggest what to convert
argument-hint: [optional focus, e.g. "hooks" or "skills"]
allowed-tools: Read, Glob, Grep, Bash(git status:*), Bash(git log:*), Bash(ls:*), Write, Edit, AskUserQuestion
---

# Reflect

Look back over this whole conversation, find what is worth keeping, and decide whether any of it should become a permanent Claude Code asset. Optional focus from the user: $ARGUMENTS

Reply in British English. Never write secrets, tokens or passwords into any file you create.

## 1. Review the session

Go back to the first user message. List:
- Procedures done more than once, or multi-step workflows that took several turns.
- Corrections, preferences and rules the user gave ("always", "don't", "use X not Y").
- Commands or checks run repeatedly.
- Manual steps that could have been automated, and mistakes that a guard would have prevented.
- Domain knowledge or project facts discovered that were not obvious.

Ignore one-off tasks and anything already visible in the code or git history. If a focus was given above, concentrate on it.

## 2. Inventory what already exists

Read these rather than guessing (skip any that are missing):
- `.claude/commands/`, `.claude/skills/`, `.claude/agents/`, `.claude/settings.json` (hooks, permissions), `.claude/settings.local.json`
- `~/.claude/commands/`, `~/.claude/skills/`, `~/.claude/agents/`, `~/.claude/settings.json`
- `CLAUDE.md` in the project and `~/.claude/CLAUDE.md`
- `.mcp.json`, plus `.githooks/` and `scripts/` for hook scripts

For each skill, agent or command, read its description, not just its name.

## 3. Compare

Mark each learning as one of: **Covered** (already handled, say by what), **Partly covered** (extend that asset), or **New**.

## 4. Pick the right kind of asset

| Kind | Use when |
|------|----------|
| Skill | A reusable multi-step procedure Claude should recognise and apply on its own |
| Slash command | A procedure the user wants to trigger by name, possibly with arguments |
| Agent (subagent) | Work that suits isolation, parallelism, a restricted tool set or a distinct persona |
| Hook | Something that must happen every time on an event; Claude "remembering" is not reliable enough |
| CLAUDE.md rule | A short standing convention or fact, not a procedure |
| MCP tool | Needs live data or an external system |
| Nothing | One-off, trivial, or already covered |

Prefer the lightest option that works. Be selective: if nothing is worth converting, say so plainly and stop.

## 5. Report

Show a table, most valuable first:

| # | Learning | Evidence from this session | Existing overlap | Recommendation | Value / effort |
|---|----------|----------------------------|------------------|----------------|----------------|

Under it, give one or two sentences per recommendation on why that asset kind fits.

## 6. Ask, then create

Use AskUserQuestion (multi-select) to ask which recommendations to create. Create nothing without approval.

For each approved item:
- Follow the style of existing files (for skills, `SKILL.md` with `name` and `description` frontmatter, as in `~/.claude/skills/session-handoff`).
- Put it at project level (`.claude/...`) unless the user says it should be personal.
- Hooks: merge into the existing `hooks` object in `.claude/settings.json`; never overwrite the existing entries.
- CLAUDE.md: add a short line outside the `AUTO` markers; never edit between them.
- Extending an existing asset: show the change and edit it in place rather than creating a duplicate.

Finish with the list of files written or changed and one line on how to use each. Do not commit unless asked.
