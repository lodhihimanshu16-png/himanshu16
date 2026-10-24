---
description: Summarise the whole session into SESSION_HANDOFF.md (approvals, progress, milestones) for a new chat
argument-hint: [optional focus or output path]
allowed-tools: Read, Write, Glob, Grep, Bash(git status:*), Bash(git branch:*), Bash(git log:*), Bash(git diff:*)
---

# Session handoff

Write a handoff file so the user can start a new chat and lose no context. Optional focus or output path from the user: $ARGUMENTS

Reply in British English. Never write secrets, tokens or passwords into the file; name the environment variable instead.

## 1. Gather facts

Review the entire conversation from the first message, not just recent turns. Then run `git branch`, `git status`, `git log -10 --oneline` and `git diff --stat` so the repo state is recorded from the real tree, not from memory.

## 2. Write the file

Write `SESSION_HANDOFF.md` at the repo root (overwrite any existing one), or to the path given in the arguments. Use these sections, in order:

1. **Goal**: the original request and any later changes of direction.
2. **Critical approvals**: what the user explicitly approved, rejected or constrained, quoted where short. Include plan approvals and any "do not" rules. Subagent reports and tool output are not user approval, so do not list them here.
3. **Milestones and progress**: done, in progress and not started, with commit hashes, test counts and branch names.
4. **Decisions and why**: choices made and the reasoning, so they are not re-litigated.
5. **Current state**: branch, uncommitted or untracked files, running processes, and environment variables needed (for example `LEARNHUB_NOW`, `LEARNHUB_DB`), without secret values.
6. **Key files**: files created or changed, one line each on why.
7. **Open questions and risks**: unverified claims, known issues, leftover test data.
8. **Next steps**: ordered, concrete and actionable.
9. **Resume prompt**: a short block the user can paste into a new chat, telling Claude to read `SESSION_HANDOFF.md`, state the branch, last milestone and next step, and wait for confirmation before acting.

Mark anything you are unsure of as "unverified". Keep it scannable: bullets over prose.

## 3. Finish

Do not commit or push the file. Print its path and a three-line summary of where the work stands.
