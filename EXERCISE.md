# Day 2 Exercise: CLAUDE.md and built-in commands

A "command" is a word that starts with `/`. You type it in the Claude Code chat to make Claude do something special.

1. Open this folder in VS Code with Claude Code. Run the app (see `README.md`).
2. Type `/init`. Claude reads the code and writes a file called `CLAUDE.md`. Open it and read it.
   `CLAUDE.md` is a set of notes. Claude reads it at the start of every chat in this folder.
3. Edit `CLAUDE.md`. Add 3 rules of your own. Examples:
   - "Use async methods in C#."
   - "Every new page goes in `src/pages`."
   - "Keep the in-memory store as the only place data lives."
4. There are three levels of memory files. Fill in the table.

   | File | Where it lives | Who sees it | Committed to git? |
   | --- | --- | --- | --- |
   | `CLAUDE.md` | Project folder | | |
   | `CLAUDE.local.md` | Project folder | | |
   | `~/.claude/CLAUDE.md` | Your home folder | | |

   Hint: project = shared with the team. Local = just you. User = all your projects.
5. Test which file wins.
   1. Put "Always reply in British English" in `~/.claude/CLAUDE.md`.
   2. Put "Reply in American English" in `CLAUDE.local.md`.
   3. Start a new chat. Ask Claude: "What is your favourite colour and why?" Look at the spelling.
   4. Remove both test lines.
6. Type `/memory`. It shows the memory files Claude loaded. Open one from the list.
7. Build a feature. Ask Claude: "Add search courses by title on the courses page (backend and frontend)."
   Check: did Claude follow your 3 rules from step 3?
8. Try two commands:
   - `/clear` starts a fresh chat.
   - `/compact` shrinks the old chat into a short summary.

   Type `/context` before and after each one. Write one line on when you would use each.
   - `/clear`: ______
   - `/compact`: ______

## What to bring to Day 3

1. Your `CLAUDE.md`.
2. One rule that Claude did not follow.
