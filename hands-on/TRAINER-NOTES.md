# Trainer notes: Day 4 hands-on (about 2h40 plus breaks)

Students follow HANDS-ON.md. The spec is FEATURE.md. Backup files are in `hands-on/solutions/`.

## Timing

| Part | Time | What happens |
| --- | --- | --- |
| Intro | 10 min | Show FEATURE.md. Explain the plan: build once with skills and hooks, once with agents |
| A (skills and hooks) | 75 min | A1 to A7, about 10 minutes each. The e2e step may take 15 |
| B (why agents) | 15 min | Read the /context table. Explain, draw the diagram |
| C (agents) | 60 min | Reset 5, C1 to C4 about 9 each (C1 and C4 take longest), compare 8, C5 bonus about 12 if time allows |

Total: about 2h40 plus breaks (10 + 75 + 15 + 60). If you run late, do C1 to C4 and show C5 as a demo (or skip it), and copy files from `solutions/` when a step stalls.

## Before the session

0. The lab files (FEATURE.md, HANDS-ON.md, `hands-on/`) are already committed on `main`. Students start from `main`, so `git status` is clean at step 0.
1. Run `dotnet test` and `npm run build` on the starting repo. Make sure ports 5080 and 5173 are free.
2. Try the first prompt once, so you know how long it takes on your machine.
3. Tell students: do not open `hands-on/`. (Claude can read it too. If it starts reading `hands-on/solutions`, tell it to ignore that folder.)
4. Windows students need Git Bash or WSL for the hook scripts.

## What to say at each step

- **Step 0.** "A branch is a safe copy of the work. We will do the same feature twice, so we need a clean start." Then "Start a fresh chat and look at /context. This is our zero."
- **A1.** "We did not write the skill. Claude did, from the rules in FEATURE.md. A skill is a recipe. Now watch Claude follow it." Point out that Claude reads the skill before touching code.
- **A2.** "A hook is not a request. It is a guard at the door. Claude cannot talk its way past it." After the block, point out that Claude found a safe way by itself.
- **A3.** "Same pattern. Create, then use." Ask: "What does this skill assume already exists?" (The cart and the quote from A1.)
- **A4.** "One test per rule. The fixed clock is why a 7-day test works today and next year." Show one test.
- **A5.** "This hook talks to Claude. The message in the hook becomes Claude's next job." Show CLAUDE.md changing after the commit. Show that the second commit does not loop.
- **A6.** "Same hook fires again. Nobody typed it." Open the Cart page in the browser if time allows.
- **A7.** "This is the whole shop trip as a script. If it passes, the feature is real." Let a failure happen if one does. Reading a FAIL and fixing it is the lesson.
- **B.** "Look at your table. It only goes up. Everything Claude reads stays in memory." Draw the diagram. "An agent is a helper with its own chat. We pay for its work, but our main chat stays small."
- **C1.** "Same job as A1 and A3 together, but a helper does it. Watch what comes back to us: a summary." Compare the /context number to A1 to A3.
- **C2 and C3.** Point out the "owns this folder only" rule. Compare with A4 and A6.
- **C4.** The reviewer cannot edit, so it can only report. Ask why that is good. This time the e2e run should all PASS. If not, show the fix going to the right agent. The commit hook fires again.
- **C5 (bonus).** "One prompt. Four helpers. Two run at the same time." Watch for the parallel start. Then compare all three runs.
- **Compare.** Let students say what they saw. Then the three-line wrap-up.

## Expected /context numbers (rough)

These are rough. They change with the model, the Claude Code version and the system prompt size. Use them only to spot a big surprise.

| Point | Roughly |
| --- | --- |
| Fresh chat | 15k to 30k tokens used |
| After A1 | 40k to 70k |
| After A4 | 80k to 120k |
| After A7 (end of Part A) | 120k to 200k, often 15% to 25% or more of the window |
| Fresh chat after /clear (Part C) | 15k to 30k |
| After C1 (all backend) | 25k to 50k (the agent's work stays out) |
| After C2 | 35k to 60k |
| After C3 | 45k to 75k |
| After C4 (end of C1 to C4, with commit) | 55k to 100k |
| C5 bonus, at the end | 55k to 100k, similar to C4 |

The point is the shape: Part A climbs steeply. Part C climbs slowly. If both are the same, check that the agents were really used (the progress lines say the agent name).

## Common failures and fixes

1. **Hook does not fire.**
   - Type `/hooks`. Is the hook listed under PreToolUse or PostToolUse with matcher Bash?
   - Check the script is executable: `ls -l .claude/hooks`. Fix with `chmod +x .claude/hooks/*.sh`.
   - Check `.claude/settings.json` is valid: `python3 -m json.tool .claude/settings.json`.
   - Close Claude Code and open it again. Settings are read at startup.
   - On Windows, run from Git Bash or WSL.
2. **The commit hook loops.**
   - The hook skips any command that contains `docs: update CLAUDE.md`. If Claude words the message differently, the hook fires again. Tell Claude: "Use exactly the message docs: update CLAUDE.md."
   - If it keeps looping, remove the PostToolUse part from `.claude/settings.json`, then add it back later.
3. **The commit hook fires on a commit that did nothing** (for example "nothing to commit"). This is fine. Claude checks `git show --stat HEAD` and will see the last real commit.
4. **The block hook stops a harmless command** (for example `echo "rm -rf"`). It checks the text of the whole command. Ask Claude to reword the command. This is a known limit.
5. **A skill is not used.** Say "Use the shop-backend skill" or type `/shop-backend`. Check that the folder name and the `name:` line match.
6. **An agent is missing in `/agents`.** Close Claude Code and open it again. Check the file is in `.claude/agents/` and has frontmatter.
7. **An agent edits outside its folder.** Check `git status`. Undo with `git checkout -- <file>`. Then tell the agent again which folder it owns, and add the rule to its file.
8. **Port in use.** Another API is still running. Find it: `lsof -i :5080`. Stop it. Never switch to port 5000. The frontend proxy expects 5080.
9. **Build errors after a skill step.** Run `cd backend && dotnet build` yourself and paste the error to Claude. Do not skip to the next step with a red build.
10. **Old tests fail after A1.** Usually the enrollment id counter or `EnrollFree` is wrong. The 13 old tests must still pass.
11. **e2e step fails on a second run.** The script uses a new student each run, so it should still pass. If revenue steps fail, restart the API and run again.
12. **Claude takes a very long time.** Let it run. If it is stuck for over 5 minutes, press Escape, then say "Continue" or copy the file from `hands-on/solutions/`.
13. **C5 reset fails because `day4-agents` has uncommitted work.** Finish the C4 commit first, then reset.
14. **Part C reset shows "would be overwritten".** Student has uncommitted work. Run `git add -A && git commit -m "save"` on day4-skills first, then redo the reset.

## After the session

Day 5 starts with this feature already built, plus bookmarks, certificates and instructor tools. Students keep their `day4-skills` and `day4-agents` branches for comparison.
