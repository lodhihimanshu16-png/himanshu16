# Day 4 Hands-On: Build "Paid Courses" with Skills, Hooks and Agents

Read FEATURE.md first. It says WHAT we build. This page says HOW we build it with Claude.

Rule for today: **nobody writes a skill, hook or agent file by hand.** You paste a "Create prompt". Claude writes the file. Then you paste a "Use prompt" and watch it work.

## Words you need

1. **Skill.** A guide in a markdown file (`.claude/skills/<name>/SKILL.md`). Claude reads it when a job matches its description. It tells Claude the steps and the files to touch.
2. **Hook.** A small script that Claude Code runs by itself at a set moment, for example before a shell command runs. Claude cannot forget or skip it.
3. **Agent (subagent).** A helper Claude with its own fresh chat, its own tools and its own rules. You give it a job. It sends back a short summary.
4. **`/context`.** A command that shows how full Claude's memory is for this chat (tokens used and a percent). Every file read, build output and test output fills it.
5. **Branch.** A separate line of work in git. We use two branches so you can do the same feature twice and compare.

## Before you start

1. Open the repo in VS Code. Open Claude Code.
2. Check the tools work: `cd backend && dotnet test` should pass (13 tests). Then `cd ../frontend && npm install && npm run build`.
3. Do not open the `hands-on/` folder. It holds backup answers for the trainer.
4. Never use port 5000. The API runs on port 5080.

## The `/context` table

After EVERY step below, type `/context`. Write the numbers here.

**Part A (main chat only)**

| Step | What we did | Tokens used | % of window |
| --- | --- | --- | --- |
| Start | New chat | | |
| A1 | shop-backend skill, used | | |
| A2 | block-dangerous hook | | |
| A3 | shop-orders skill, used | | |
| A4 | shop-tests skill, used | | |
| A5 | commit hook | | |
| A6 | shop-frontend skill, used | | |
| A7 | e2e-check skill, used | | |

**Part C (agents, one at a time)**

| Step | What we did | Tokens used | % of window |
| --- | --- | --- | --- |
| Start | New chat after `/clear` | | |
| C1 | backend-dev, used (all backend) | | |
| C2 | test-writer, used (all tests) | | |
| C3 | frontend-dev, used (all screens) | | |
| C4 | reviewer, used, then commit | | |

**Bonus (C5, one prompt)**

| Step | What we did | Tokens used | % of window |
| --- | --- | --- | --- |
| Start | New chat on a fresh branch | | |
| C5 | One orchestration prompt, then commit | | |

---

# Part A. Skills and hooks, in the main chat (75 min)

No agents in this part. One chat does everything. Watch the numbers grow.

## Step 0. Make a branch

1. `git checkout -b day4-skills`
2. `git status`. It should say the tree is clean. If not: `git add -A` then `git commit -m "start of lab"`.
3. Start a fresh Claude Code chat. Type `/context` and write the first row.

## A1. Skill: shop-backend

**Create prompt**

```text
Create a project skill in .claude/skills/shop-backend/SKILL.md. Only write the skill file. Do not build the feature yet.

The skill is for building the shopping part of the LearnHub backend: course prices, the cart, the 15% bundle discount and coupons.
First read FEATURE.md rules R1 to R14, CLAUDE.md and the existing skill .claude/skills/add-api-endpoint/SKILL.md. Match that skill's style.

The file needs frontmatter with name: shop-backend and a description that says what the skill does and when to use it.
Then write numbered steps that point to the real files:
1. Price on Models/Course.cs and on the six seed courses (see Starting data in FEATURE.md), and in CourseEndpoints.
2. New models CartItem, Coupon and Order (with OrderLine and PriceQuote) in Models/.
3. InMemoryStore: a clock the tests can replace (Func<DateTime>), Cart, Orders and Coupons lists with the 5 seed coupons, an enrollment id counter (ids must never be reused), EnrollFree (paid courses refused), cart methods, a Quote method (bundle first, then coupon, round with MidpointRounding.AwayFromZero), and coupon checks.
4. CartEndpoints.cs and CheckoutEndpoints.cs (preview only) with the right 400 / 409 / 201 / 204 answers, registered in Program.cs.
5. Run dotnet build and dotnet test.
Say which FEATURE.md rule each step covers. Say that checkout, orders and refunds are NOT part of this skill.
```

**Use prompt**

```text
Use the shop-backend skill to build FEATURE.md rules R1 to R14 in the backend. Then run dotnet build and dotnet test.
```

**What you should see**

1. Claude says it is using the skill, then reads FEATURE.md and a few backend files.
2. New files in `Models/` and `Endpoints/`. `InMemoryStore.cs` grows.
3. `dotnet build` has 0 errors. The 13 old tests still pass.
4. A short summary of the files changed.

If Claude does not mention the skill, type `/shop-backend` or say "Use the shop-backend skill" again.

**Check**

1. `cd backend && dotnet build && dotnet test`
2. `git status` shows new files.
3. Type `/context`. Fill the table.

## A2. Hook 1: block dangerous commands

**Create prompt**

```text
Create a Claude Code hook that blocks dangerous shell commands.
1. Write the script .claude/hooks/block-dangerous.sh. Claude Code sends it JSON on stdin. The command is in tool_input.command. Do not use jq. Use sed or grep.
2. The script must block these, with exit code 2 and a short plain-English reason on stderr:
   rm -rf, git push --force or -f, git reset --hard, git clean -f (any form, such as -fd), npm publish, curl piped into sh or bash, deleting appsettings files.
   Everything else must exit 0.
3. Make the script executable with chmod +x.
4. Register it in .claude/settings.json as a PreToolUse hook with matcher "Bash". The command is: bash "$CLAUDE_PROJECT_DIR/.claude/hooks/block-dangerous.sh"
5. Test the script by piping sample JSON into it. Show me each result.
Do not touch any other file.
```

**Use prompt**

```text
Delete the bin and obj folders with rm -rf
```

**What you should see**

1. Claude tries `rm -rf`. The hook blocks it. Claude shows the reason.
2. Claude picks a safe way, for example `dotnet clean`, or deleting named files.

If the hook does not fire: type `/hooks`. Is a PreToolUse hook for Bash listed? If not, close Claude Code and open it again. Then try again.

**Check**

1. `cat .claude/settings.json` shows the hook.
2. `ls -l .claude/hooks` shows the script is executable (`x`).
3. Run `/context`. Fill the table.

## A3. Skill: shop-orders

**Create prompt**

```text
Create a project skill in .claude/skills/shop-orders/SKILL.md. Only write the skill file.

The skill is for building checkout, orders, refunds and instructor revenue in the LearnHub backend.
First read FEATURE.md rules R15 to R20, CLAUDE.md, and the skill .claude/skills/shop-backend/SKILL.md. Match its style.

The file needs frontmatter with name: shop-orders and a description that says what it does and when to use it.
Write numbered steps that point to the real files:
1. InMemoryStore.Checkout: check name, empty cart, courses already owned, then Quote. Save the Order, count the coupon use, enroll in every course, empty the cart.
2. InMemoryStore.GetOrders (newest first).
3. InMemoryStore.RefundOrder: the error texts in order, the 7 day limit (exactly 7 days is fine), progress under 30% on every course using the real ratio, then the refund effects (status, enrollments, progress, coupon use back).
4. InMemoryStore.GetInstructorRevenue: gross, refunds, 70% share of net.
5. POST /api/checkout, OrderEndpoints.cs, RevenueEndpoints.cs, with the right 200 / 201 / 400 / 404 / 409 answers, registered in Program.cs.
6. dotnet build and dotnet test.
Say which rule each step covers. Say that the skill needs the shop-backend work done first.
```

**Use prompt**

```text
Use the shop-orders skill to build FEATURE.md rules R15 to R20 in the backend. Then run dotnet build and dotnet test.
```

**What you should see**

1. Claude uses the skill. New `OrderEndpoints.cs` and `RevenueEndpoints.cs`. `Checkout`, `RefundOrder` and `GetInstructorRevenue` appear in the store.
2. Build has 0 errors. The 13 old tests still pass.

**Check**

1. `cd backend && dotnet build && dotnet test`
2. `/context`. Fill the table.

## A4. Skill: shop-tests

**Create prompt**

```text
Create a project skill in .claude/skills/shop-tests/SKILL.md. Only write the skill file.

The skill is for writing xUnit tests for the LearnHub shop: one test for each rule R1 to R20 in FEATURE.md.
First read FEATURE.md, .claude/skills/write-api-tests/SKILL.md and backend/LearnHub.Tests/InMemoryStoreTests.cs. Match their style.

The file needs frontmatter with name: shop-tests and a description that says what it does and when to use it.
Write numbered steps:
1. Create ShopTests.cs in backend/LearnHub.Tests.
2. Use a fixed clock: a fixed Today date and a NewStore() helper that sets store.Clock. Never use DateTime.UtcNow in a test. To move time, set store.Clock again inside the test.
3. Name each test after its rule (for example R6_Bundle_...) and put a comment with the rule number above it.
4. Use the numbers from the worked examples in FEATURE.md (76.50, 97.75, the 40% refund, Liam Carter's share).
5. Only edit files in backend/LearnHub.Tests. If a test fails because of an app bug, report it. Do not change app code.
6. Run dotnet test and report how many passed.
```

**Use prompt**

```text
Use the shop-tests skill to write tests for every rule R1 to R20. Then run dotnet test.
```

**What you should see**

1. A new `ShopTests.cs`. About 23 new tests.
2. `dotnet test` passes: the 13 old tests plus the new ones.
3. If a test fails, Claude finds out if the test or the app is wrong and tells you.

**Check**

1. `cd backend && dotnet test`
2. `grep -o "R[0-9]*_" backend/LearnHub.Tests/ShopTests.cs | sort -u | wc -l`. It should print 20, one for every rule R1 to R20.
3. `/context`. Fill the table.

## A5. Hook 2: update CLAUDE.md after every commit

This hook keeps CLAUDE.md true. After Claude commits, the hook tells Claude to update CLAUDE.md.

**Create prompt**

```text
Create a Claude Code hook that keeps CLAUDE.md up to date after commits.
1. Write the script .claude/hooks/after-commit-update-claude-md.sh. It reads JSON on stdin. The command is in tool_input.command. Do not use jq.
2. If the command contains "git commit" and does NOT contain "docs: update CLAUDE.md", print this to stderr and exit with code 2:
   You just made a commit. Run git show --stat HEAD, update CLAUDE.md (folder map, endpoints, rules) to match, then commit only CLAUDE.md with the message 'docs: update CLAUDE.md'.
   In every other case exit 0.
3. Make it executable with chmod +x.
4. Add it to .claude/settings.json as a PostToolUse hook with matcher "Bash". Keep the PreToolUse hook that is already there. The command is: bash "$CLAUDE_PROJECT_DIR/.claude/hooks/after-commit-update-claude-md.sh"
5. Test the script by piping sample JSON into it: a normal commit (expect 2), the CLAUDE.md commit (expect 0), and "ls" (expect 0).
Do not commit anything.
```

If the new hook is not active, type `/hooks` to check. Restart Claude Code if needed.

**Use prompt**

```text
Commit everything so far (code, skills, hooks).
```

**What you should see**

1. Claude runs `git add` and `git commit`.
2. The hook speaks up: "You just made a commit. Run git show --stat HEAD ...".
3. Claude runs `git show --stat HEAD`, updates CLAUDE.md (new folders, endpoints, rules), and commits it with the message `docs: update CLAUDE.md`.
4. That second commit does NOT trigger the hook again.

**Check**

1. `git log --oneline -3` shows two new commits.
2. `git show --stat HEAD` shows only `CLAUDE.md` changed.
3. `/context`. Fill the table.

## A6. Skill: shop-frontend

**Create prompt**

```text
Create a project skill in .claude/skills/shop-frontend/SKILL.md. Only write the skill file.

The skill is for building the shop screens in the LearnHub React frontend.
First read the Screens and API parts of FEATURE.md, CLAUDE.md and .claude/skills/add-react-page/SKILL.md. Match its style.

The file needs frontmatter with name: shop-frontend and a description that says what it does and when to use it.
Write numbered steps that point to the real files:
1. api.js: formatPrice and the cart, preview, checkout, orders, refund and revenue calls, all with request().
2. CourseCard.jsx: price or "Free", and an "Add to cart" button for paid courses (the card is a link, so stop the click from opening the course).
3. CoursesPage.jsx and CourseDetailPage.jsx.
4. CartPage.jsx at /cart: coupon box, price breakdown from the preview call (the page does no price maths), Remove and Pay buttons.
5. MyOrdersPage.jsx at /orders: Refund button, and the reason under the order if the refund is refused.
6. InstructorRevenuePage.jsx at /instructor-revenue.
7. Routes in App.jsx, links in NavBar.jsx, a few styles at the end of index.css.
8. npm run build.
Say the skill must not touch backend/.
```

**Use prompt**

```text
Use the shop-frontend skill to build the shop screens from FEATURE.md. Then run npm run build. When it passes, commit the frontend.
```

**What you should see**

1. New pages and a changed course card. `npm run build` passes.
2. The commit happens, and the CLAUDE.md hook fires again. Claude updates CLAUDE.md and commits it.

**Check**

1. `cd frontend && npm run build`
2. `git log --oneline -4`
3. `/context`. Fill the table.

## A7. Skill: e2e-check

This skill checks the whole shop trip with a script.

**Create prompt**

```text
Create two files.

1. A bash script scripts/e2e-shop.sh. It uses curl against the running API (default http://localhost:5080, the base URL can be the first argument). Never use port 5000.
   It uses a new student name each run. It prints "PASS  step name" or "FAIL  step name (reason)" for each step, and exits with a non-zero code if any step fails.
   Steps: courses show a price; add courses 2, 3 and 4 to the cart; adding the same course twice gives 409; a free course gives 400; preview with coupon welcome10 gives total 76.50; an expired coupon is explained; checkout with an empty cart gives 400; checkout gives 201; the cart is empty; the order is listed; the student is enrolled; the same coupon cannot be used twice; instructor revenue goes up by the right amount; refund gives 200; enrollments are gone; a second refund gives 409; a refund at 40% progress gives 400.
   Do not use jq.
2. A project skill .claude/skills/e2e-check/SKILL.md with frontmatter name: e2e-check and a description that says what it does and when to use it. Steps: make sure the API runs on port 5080 (start it in the background if not), run the script, read any FAIL, fix the code (not the script), run again, stop the API, report.

Do not run anything yet.
```

**Use prompt**

```text
Start the API, run e2e-check, fix anything that fails.
```

**What you should see**

1. Claude starts the API in the background, runs the script and shows a list of PASS lines.
2. If something fails, Claude reads the reason, fixes the code, restarts the API and runs again.
3. At the end Claude stops the API.

**Check**

1. Last line says `All steps passed.`
2. Nothing is left running on port 5080: `curl -s localhost:5080/api/courses` shows nothing.
3. `/context`. Fill the table. Commit if there are changes.

You now have the full feature, built with 5 skills and 2 hooks. Keep the Part A table. We will compare soon.

---

# Part B. Why agents? (15 min)

Look at your Part A table. The numbers only go up. Why?

## What fills the memory

In the main chat, Claude keeps EVERYTHING it sees:

1. Every file it read.
2. Every build output and every test output.
3. Every try, every fix, every long list.

Memory is limited. When it fills up, Claude gets slower, forgets early details and costs more.

## What an agent does

An agent is a helper with its own fresh chat. It reads the files, runs the builds, and fights with errors in ITS chat. When it is done, it sends back a short summary. Your main chat only keeps the summary.

```text
WITHOUT AGENTS (Part A)               WITH AGENTS (Part C)

 Main chat                             Main chat
 +---------------------------+         +---------------------------+
 | your prompt               |         | your prompt               |
 | file read   (long)        |         | summary from backend-dev  |
 | file read   (long)        |         | summary from frontend-dev |
 | build output (long)       |         | summary from test-writer  |
 | test output (long)        |         | summary from reviewer     |
 | fix, build, test again... |         +---------------------------+
 | frontend files (long)     |           small and tidy
 | e2e output (long)         |
 +---------------------------+           Each agent has its own chat:
   big and growing                       +-------------+ +-------------+
                                         | backend-dev | | test-writer | ...
                                         | (files,     | | (files,     |
                                         |  builds)    | |  tests)     |
                                         +-------------+ +-------------+
                                           thrown away when done
```

Extra gain: agents that do not need each other can work at the same time. That is called "in parallel".

Skills and hooks still work. We keep them. Agents use the skills.

---

# Part C. The same feature, with agents (60 min)

We build the SAME full feature again on a new branch. This time, helpers do the heavy work. Then the /context numbers can be compared fairly with Part A.

## Reset

1. `git checkout main`
2. `git checkout -b day4-agents`
3. `git checkout day4-skills -- .claude scripts`
   This brings back your skills, hooks, settings and the e2e script, but not the feature code. The files are ready to commit. Do not commit them yet.
4. Start a fresh chat: type `/clear`. Then `/context` and write the first row of the Part C table.

Both hooks are still on. The agents run shell commands too, so the safety hook protects them.

## C1. Agent: backend-dev

**Create prompt**

```text
Create a subagent in .claude/agents/backend-dev.md that builds backend features for LearnHub.
A subagent file has frontmatter with name, description (what it does and when to use it), tools and model, then instructions.
- name: backend-dev. tools: Read, Grep, Glob, Edit, Write, Bash. model: sonnet.
- It owns backend/LearnHub.Api only. It must not touch frontend/, backend/LearnHub.Tests or docs.
- It reads CLAUDE.md and the FEATURE.md rules it is given.
- For the shop it follows .claude/skills/shop-backend/SKILL.md, then .claude/skills/shop-orders/SKILL.md. For other work it follows .claude/skills/add-api-endpoint/SKILL.md.
- It keeps logic in InMemoryStore and endpoints thin, follows any API contract it is given exactly, runs dotnet build, and does not write tests.
- It reports back in under 12 lines: files changed, each route with its request and response shape and error cases, and the new store method names.
Look at an existing file in .claude/skills for the markdown style. Only write this one file.
```

Type `/agents` to see it. If it is missing, restart Claude Code.

**Use prompt**

```text
Use the backend-dev agent to build ALL the backend for FEATURE.md rules R1 to R20 (prices, cart, bundle, coupons, checkout, orders, refunds, instructor revenue). Reply with only its summary and the build result.
```

**What you should see**

1. Claude hands the job to `backend-dev`. You see a progress line, not every file.
2. A short summary comes back. The feature code is in the files.
3. `cd backend && dotnet build && dotnet test` passes (13 old tests).

**Check:** `/context`. Fill the table. Compare with A1 to A3 together.

## C2. Agent: test-writer

**Create prompt**

```text
Create a subagent in .claude/agents/test-writer.md that writes xUnit tests for LearnHub.
- name: test-writer. tools: Read, Grep, Glob, Edit, Write, Bash. model: sonnet.
- It owns backend/LearnHub.Tests only.
- It reads the new store methods and the FEATURE.md rules it is given.
- For the shop it follows .claude/skills/shop-tests/SKILL.md (one test per rule, with a fixed clock). For other work it follows .claude/skills/write-api-tests/SKILL.md.
- It runs dotnet test. If a test fails because of an app bug, it reports the bug and does not change app code.
- It reports back in under 10 lines: test files, number of tests added, pass and fail count, any rule without a test, any bug found.
Only write this one file.
```

**Use prompt**

```text
Use the test-writer agent to write tests for every rule R1 to R20 and run dotnet test. Reply with only its summary.
```

**What you should see:** a summary like "23 tests added, all pass". A new `ShopTests.cs`.

**Check:** `cd backend && dotnet test`. Then `/context`. Fill the table. Compare with A4.

## C3. Agent: frontend-dev

**Create prompt**

```text
Create a subagent in .claude/agents/frontend-dev.md that builds frontend features for LearnHub.
- name: frontend-dev. tools: Read, Grep, Glob, Edit, Write, Bash. model: sonnet.
- It owns frontend/ only. It must not touch backend/, tests or docs. It must not read backend code.
- It works from the API table in FEATURE.md or a contract it is given.
- For the shop it follows .claude/skills/shop-frontend/SKILL.md. For other pages it follows .claude/skills/add-react-page/SKILL.md.
- Every backend call goes in src/api.js. Errors show with <p className="message error">. It runs npm run build.
- It reports back in under 12 lines: files changed, routes and nav links, api.js functions added.
Only write this one file.
```

**Use prompt**

```text
Use the frontend-dev agent to build all the shop screens from FEATURE.md (course card and course page price, Cart, My Orders, Instructor Revenue, nav links). Reply with only its summary.
```

**What you should see:** a summary. `npm run build` passes.

**Check:** `cd frontend && npm run build`. Then `/context`. Fill the table. Compare with A6.

## C4. Agent: reviewer

**Create prompt**

```text
Create a subagent in .claude/agents/reviewer.md that reviews LearnHub changes. It must never edit files.
- name: reviewer. tools: Read, Grep, Glob, Bash only. model: sonnet.
- It runs git status and git diff HEAD, and reads FEATURE.md and CLAUDE.md.
- It checks the rules against the code, and the CLAUDE.md coding rules.
- It runs dotnet build, dotnet test and npm run build.
- It follows .claude/skills/e2e-check/SKILL.md: starts the API, runs scripts/e2e-shop.sh, stops the API. It only reports. It does not fix.
- It reports in under 15 lines: build, test and e2e results, then bugs, missing rules, style, missing tests, each with a file name.
Only write this one file.
```

**Use prompt**

```text
Use the reviewer agent to run e2e-check and review the work. Reply with only its report. If anything fails, send the fix to the right agent (backend-dev, frontend-dev or test-writer) and run the reviewer again. When everything passes, commit the work.
```

**What you should see**

1. The reviewer reports build, tests and e2e. Expected: every step PASS.
2. If something fails, the right agent fixes it and the reviewer runs again.
3. Claude commits. The CLAUDE.md hook fires. Claude updates CLAUDE.md and commits it.

**Check**

1. `git log --oneline -4` and `git status`. The reviewer itself edited nothing.
2. `/context`. Fill the table. Compare the end of C4 with the end of A7.

## C5. Bonus: one prompt does it all

Now the same feature a third time, with ONE prompt that uses all four helpers.

**Reset**

1. `git checkout main`
2. `git checkout -b day4-orchestrated`
3. `git checkout day4-agents -- .claude scripts`
4. `/clear`, then `/context`. Write the first row of the Bonus table.

**Use prompt**

```text
Build the "Paid Courses" feature from FEATURE.md (rules R1 to R20, the API table and the Screens), using the agents. Work in this order.
1. Use backend-dev to build the whole backend. Wait for it.
2. Then use frontend-dev AND test-writer at the same time (in parallel):
   - frontend-dev: all the shop screens, using the API table in FEATURE.md.
   - test-writer: tests for every rule R1 to R20.
3. Then use the reviewer agent. If it reports a problem, send the fix to the right agent, then run the reviewer again.
4. When the reviewer says everything passes, commit the work.
Give me only a short summary of what each agent reported.
```

**What you should see**

1. Claude starts `backend-dev`, then two agents at once.
2. The reviewer runs the e2e check. Every step is PASS.
3. Claude commits. The CLAUDE.md hook fires and CLAUDE.md is updated.

**Check:** build, tests and `npm run build` pass. `/context`. Fill the Bonus table.

## Compare

Fill this in from your tables.

| | Part A (skills, one chat) | C1 to C4 (agents, one at a time) | C5 (one prompt) |
| --- | --- | --- | --- |
| Tokens used at the end | | | |
| % of window at the end | | | |
| Biggest jump in one step | | | |

Questions to discuss:

1. Which step grew the main chat the most in Part A? What did the same job cost in C1 to C4?
2. Did the agents stay inside their own folders?
3. What was harder with agents?
4. What did the one-prompt version save, and what did it cost you in control?

## Wrap up in 3 lines

1. **Skill:** use it to teach Claude HOW to do a job in your project, so every run follows the same steps.
2. **Hook:** use it for things that must ALWAYS happen or NEVER happen, because Claude cannot forget a hook.
3. **Agent:** use it for a big job, so the long working notes stay out of your main chat. Several agents can work at the same time.
