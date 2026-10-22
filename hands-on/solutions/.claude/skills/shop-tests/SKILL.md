---
name: shop-tests
description: Writes xUnit tests for the LearnHub shop, one test per rule in FEATURE.md (R1 to R20), using a fixed clock so dates never change. Use after the shop backend exists and the user asks for tests or coverage of the shop.
---

# Shop tests

Work in `backend/LearnHub.Tests`. Read `FEATURE.md`, `.claude/skills/write-api-tests/SKILL.md` and `InMemoryStoreTests.cs` first. Copy their style.
Only edit files in `backend/LearnHub.Tests`. If a test fails because of a bug in the app, report the bug. Do not change app code.

## Steps

1. Create `ShopTests.cs` with `using LearnHub.Api.Data;` and `using LearnHub.Api.Models;`, namespace `LearnHub.Tests`, class `ShopTests`.
2. Make the clock fixed.
   - `private static readonly DateTime Today = new(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc);`
   - A helper `NewStore()` that creates `new InMemoryStore()` and sets `store.Clock = () => Today;`.
   - To move time, set `store.Clock = () => Today.AddDays(8);` inside the test.
   - Never use `DateTime.UtcNow` in a test.
3. Add small helpers: `AddToCart(store, name, params int[] courseIds)` and `CompleteLessons(store, name, courseId, count)`.
4. Write at least one test for every rule. Name it after the rule, for example `R6_Bundle_GivesFifteenPercentOff_OnlyForThreeOrMoreCourses`. Put a comment `// R6` above it.
5. Use the numbers in the worked examples of FEATURE.md. Seed prices: course 2 = 40, 3 = 30, 4 = 30, 6 = 60. Courses 1 and 5 are free.
   - R6 and R10: courses 2, 3, 4 with `WELCOME10` give subtotal 100, bundle 15, coupon 8.50, total 76.50.
   - R11: courses 2, 3, 6 with `CLOUD25` give total 97.75.
   - R12: course 3 with `FLAT50` gives coupon 30 and total 0.
   - R13: add a test course with price 0.01 and a 50% coupon through `store.Courses.Add(...)` and `store.Coupons.Add(...)`. A half cent rounds up to 0.01.
   - R8: use `OLD5` for expired. Add your own coupon for "last day still works" and "used up".
   - R18: course 4 has 5 lessons. Finishing 2 is 40% and must be refused. Also test day 7 (allowed), day 8 (refused), a second refund, and an unknown order.
   - R19: after a refund there are no enrollments or progress for that student, and the coupon `UsedCount` is back to 0.
   - R20: Liam Carter's share after the example order is 70% of the net sales.
6. Also test that enrollment ids stay unique after a refund.
7. Run: `cd backend && dotnet test`. Fix failing tests that are wrong. Do not delete a test just to make it pass.
8. Report how many tests passed in total and how many are new. List any rule that has no test.
