# Solutions (backup for the trainer)

These files are what each "Create prompt" in HANDS-ON.md should produce. They mirror the final paths.

**Only copy a file if a live step fails.** Students should let Claude write these files. Copy one only when Claude's version does not work and you are running out of time.

## How to copy a file

1. Open a terminal in the repo root.
2. Copy the file you need. Example for the first skill:
   `mkdir -p .claude/skills/shop-backend && cp hands-on/solutions/.claude/skills/shop-backend/SKILL.md .claude/skills/shop-backend/`
3. For hooks, also run `chmod +x .claude/hooks/*.sh`.
4. For settings, there are two files. At A2 copy `settings.A2.json` as `.claude/settings.json` (it has only the PreToolUse hook). At A5 copy `settings.json` over it (it has both hooks). Do not copy `settings.json` at A2: the A5 hook script would be missing.
5. Type `/hooks` in Claude Code. If the hook is not listed, close Claude Code and open it again.

## What is here

| Step | File |
| --- | --- |
| A1 | `.claude/skills/shop-backend/SKILL.md` |
| A2 | `.claude/hooks/block-dangerous.sh` and `.claude/settings.A2.json` (copy it as `.claude/settings.json`) |
| A3 | `.claude/skills/shop-orders/SKILL.md` |
| A4 | `.claude/skills/shop-tests/SKILL.md` |
| A5 | `.claude/hooks/after-commit-update-claude-md.sh` and `.claude/settings.json` (both hooks) |
| A6 | `.claude/skills/shop-frontend/SKILL.md` |
| A7 | `.claude/skills/e2e-check/SKILL.md` and `scripts/e2e-shop.sh` |
| C1 | `.claude/agents/backend-dev.md` |
| C2 | `.claude/agents/test-writer.md` |
| C3 | `.claude/agents/frontend-dev.md` |
| C4 | `.claude/agents/reviewer.md` |

The feature code itself (the backend and frontend) is not here. Claude writes it from the skills.
