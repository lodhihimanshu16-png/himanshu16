---
name: e2e-check
description: Runs a full end-to-end check of the LearnHub shop against the running API with scripts/e2e-shop.sh (cart, coupon, checkout, orders, enrollment, refund, revenue). Use when the user asks to check that the whole shop works, before a commit, or after big changes.
---

# End-to-end check of the shop

The check is a bash script: `scripts/e2e-shop.sh`. It uses `curl` against the real API and prints PASS or FAIL for each step.

## Steps

1. If `scripts/e2e-shop.sh` does not exist, create it.
   - Start with `#!/usr/bin/env bash`. The base URL is the first argument, default `http://localhost:5080`. Never use port 5000.
   - Use a new student name each run, for example `E2E-$$-$(date +%s)`.
   - Helpers: `call METHOD PATH [JSON]` sets `STATUS` and `BODY`; `expect_status` and `expect_body` print `PASS  name` or `FAIL  name (why)` and count the failures.
   - Steps, in order: courses show a price; add courses 2, 3 and 4 to the cart (201); same course twice (409); free course 1 (400); cart lists three; preview with `welcome10` shows subtotal 100, bundle 15, coupon 8.50, total 76.50; expired coupon `OLD5` is explained; empty checkout (400); checkout (201); cart empty; order is listed; student is enrolled; owned course cannot be added (409); same coupon twice (400); instructor revenue goes up by 30.60 and the share by 21.42 (compare before and after, so a second run still works); refund (200); enrollments gone; second refund (409); unknown order (404); revenue share back to where it was; a student with 40% progress on course 4 is refused a refund (400).
   - End with exit code 0 if every step passed, otherwise 1.
2. Make sure the API is running on port 5080: `curl -s localhost:5080/api/courses`. If not, start it in the background: `cd backend/LearnHub.Api && dotnet run` (run in the background), and wait until the curl works.
3. Run: `bash scripts/e2e-shop.sh`.
4. If a step says FAIL, read the reason. Find the cause in the code and fix the code. Do not change the script to hide a real problem. Restart the API after backend changes. Run again.
5. Stop the API when you are done.
6. Report: how many steps passed, which failed, and what you fixed.
