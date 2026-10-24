# Instructor payout reconciliation

Prepared for: Finance
Scope: orders created 2025-10-01 to 2026-09-30 (twelve months to the dataset's "today", 1 October 2026), 70% instructor share of net sales (sales after refunds).

## Summary

- **Total over-paid: 7,639.04** across 139 instructor-months and all 12 instructors. Every instructor was over-paid.
- **Nobody was under-paid.** 0 instructor-months have a negative difference. 91 were over-paid and 48 have a difference of 0.00.
- **Reported 61,663.66 against correct 54,024.62.** Reported is 70% of every order line in scope, including refunded orders. Correct is 70% of paid orders only.
- **Refunded line gross in scope is 10,912.87**, whose 70% is 7,639.01. The 0.03 gap to 7,639.04 is per-row rounding: each instructor-month's share is rounded to pennies before the difference is taken, so the rounding does not cancel exactly.
- **Two instructor-months had only refunded sales:** Grace Okafor in 2026-02 and Daniel Fischer in 2026-06. Their correct payout is 0.00, so neither should have a row. Together they account for 30.80 and 34.30 of the total.
- **Cause and status:** `PayoutReportBuilder.Build` did not filter on `orders.status`, so refunded sales were paid out at 70%. Fixed in commit `dfa2c3c`, with tests in `6eb2916`.

## Per-instructor totals

| Instructor | Months | Reported share | Correct share | Difference |
| --- | --- | --- | --- | --- |
| Maya Rao (1) | 12 | 5,818.58 | 4,983.65 | 834.93 |
| Liam Carter (2) | 12 | 8,013.45 | 6,950.47 | 1,062.98 |
| Sofia Alvarez (3) | 12 | 8,796.90 | 8,026.15 | 770.75 |
| Ken Watanabe (4) | 12 | 7,698.40 | 6,713.20 | 985.20 |
| Priya Nair (5) | 12 | 9,793.98 | 8,598.58 | 1,195.40 |
| Omar Hassan (6) | 12 | 6,432.65 | 5,474.60 | 958.05 |
| Ana Ruiz (7) | 12 | 3,707.20 | 3,133.31 | 573.89 |
| Grace Okafor (8) | 11 | 1,694.03 | 1,532.33 | 161.70 |
| Daniel Fischer (9) | 9 | 919.79 | 835.75 | 84.04 |
| Mei Lin (10) | 11 | 1,727.28 | 1,618.26 | 109.02 |
| Carlos Mendes (11) | 12 | 2,628.31 | 2,327.69 | 300.62 |
| Hannah Cohen (12) | 12 | 4,433.09 | 3,830.63 | 602.46 |

## Full reconciliation by instructor and month

| Instructor | Month | Reported share | Correct share | Difference |
| --- | --- | --- | --- | --- |
| Maya Rao | 2025-10 | 217.32 | 188.16 | 29.16 |
| Maya Rao | 2025-11 | 980.82 | 846.91 | 133.91 |
| Maya Rao | 2025-12 | 331.00 | 331.00 | 0.00 |
| Maya Rao | 2026-01 | 1,074.61 | 887.96 | 186.65 |
| Maya Rao | 2026-02 | 697.60 | 628.19 | 69.41 |
| Maya Rao | 2026-03 | 245.67 | 211.37 | 34.30 |
| Maya Rao | 2026-04 | 340.18 | 284.88 | 55.30 |
| Maya Rao | 2026-05 | 973.53 | 810.71 | 162.82 |
| Maya Rao | 2026-06 | 335.37 | 285.60 | 49.77 |
| Maya Rao | 2026-07 | 240.52 | 156.07 | 84.45 |
| Maya Rao | 2026-08 | 102.90 | 102.90 | 0.00 |
| Maya Rao | 2026-09 | 279.06 | 249.90 | 29.16 |
| Liam Carter | 2025-10 | 380.74 | 339.68 | 41.06 |
| Liam Carter | 2025-11 | 806.53 | 658.25 | 148.28 |
| Liam Carter | 2025-12 | 756.55 | 611.65 | 144.90 |
| Liam Carter | 2026-01 | 1,610.22 | 1,357.59 | 252.63 |
| Liam Carter | 2026-02 | 251.83 | 217.53 | 34.30 |
| Liam Carter | 2026-03 | 780.73 | 663.83 | 116.90 |
| Liam Carter | 2026-04 | 441.88 | 359.28 | 82.60 |
| Liam Carter | 2026-05 | 1,532.80 | 1,417.64 | 115.16 |
| Liam Carter | 2026-06 | 424.81 | 362.51 | 62.30 |
| Liam Carter | 2026-07 | 416.29 | 392.49 | 23.80 |
| Liam Carter | 2026-08 | 327.43 | 327.43 | 0.00 |
| Liam Carter | 2026-09 | 283.64 | 242.59 | 41.05 |
| Sofia Alvarez | 2025-10 | 330.44 | 254.03 | 76.41 |
| Sofia Alvarez | 2025-11 | 1,354.58 | 1,155.93 | 198.65 |
| Sofia Alvarez | 2025-12 | 705.64 | 663.64 | 42.00 |
| Sofia Alvarez | 2026-01 | 1,537.82 | 1,404.92 | 132.90 |
| Sofia Alvarez | 2026-02 | 476.99 | 446.19 | 30.80 |
| Sofia Alvarez | 2026-03 | 561.18 | 561.18 | 0.00 |
| Sofia Alvarez | 2026-04 | 406.28 | 406.28 | 0.00 |
| Sofia Alvarez | 2026-05 | 1,189.87 | 994.36 | 195.51 |
| Sofia Alvarez | 2026-06 | 998.66 | 977.66 | 21.00 |
| Sofia Alvarez | 2026-07 | 247.88 | 247.88 | 0.00 |
| Sofia Alvarez | 2026-08 | 452.30 | 452.30 | 0.00 |
| Sofia Alvarez | 2026-09 | 535.26 | 461.78 | 73.48 |
| Ken Watanabe | 2025-10 | 555.97 | 453.66 | 102.31 |
| Ken Watanabe | 2025-11 | 1,117.94 | 974.89 | 143.05 |
| Ken Watanabe | 2025-12 | 858.37 | 756.06 | 102.31 |
| Ken Watanabe | 2026-01 | 1,415.57 | 1,210.78 | 204.79 |
| Ken Watanabe | 2026-02 | 255.08 | 255.08 | 0.00 |
| Ken Watanabe | 2026-03 | 478.28 | 450.98 | 27.30 |
| Ken Watanabe | 2026-04 | 416.27 | 367.97 | 48.30 |
| Ken Watanabe | 2026-05 | 1,082.08 | 958.85 | 123.23 |
| Ken Watanabe | 2026-06 | 451.97 | 396.67 | 55.30 |
| Ken Watanabe | 2026-07 | 591.97 | 413.36 | 178.61 |
| Ken Watanabe | 2026-08 | 225.05 | 225.05 | 0.00 |
| Ken Watanabe | 2026-09 | 249.85 | 249.85 | 0.00 |
| Priya Nair | 2025-10 | 1,609.66 | 1,505.95 | 103.71 |
| Priya Nair | 2025-11 | 2,077.39 | 1,876.11 | 201.28 |
| Priya Nair | 2025-12 | 761.80 | 595.44 | 166.36 |
| Priya Nair | 2026-01 | 1,446.82 | 1,196.31 | 250.51 |
| Priya Nair | 2026-02 | 475.89 | 337.88 | 138.01 |
| Priya Nair | 2026-03 | 452.27 | 417.97 | 34.30 |
| Priya Nair | 2026-04 | 255.85 | 255.85 | 0.00 |
| Priya Nair | 2026-05 | 891.05 | 734.82 | 156.23 |
| Priya Nair | 2026-06 | 379.83 | 379.83 | 0.00 |
| Priya Nair | 2026-07 | 434.57 | 323.87 | 110.70 |
| Priya Nair | 2026-08 | 348.71 | 348.71 | 0.00 |
| Priya Nair | 2026-09 | 660.14 | 625.84 | 34.30 |
| Omar Hassan | 2025-10 | 105.91 | 105.91 | 0.00 |
| Omar Hassan | 2025-11 | 627.84 | 627.84 | 0.00 |
| Omar Hassan | 2025-12 | 186.90 | 124.60 | 62.30 |
| Omar Hassan | 2026-01 | 264.31 | 222.31 | 42.00 |
| Omar Hassan | 2026-02 | 232.86 | 232.86 | 0.00 |
| Omar Hassan | 2026-03 | 562.10 | 520.10 | 42.00 |
| Omar Hassan | 2026-04 | 639.77 | 577.47 | 62.30 |
| Omar Hassan | 2026-05 | 1,406.12 | 1,222.44 | 183.68 |
| Omar Hassan | 2026-06 | 379.86 | 379.86 | 0.00 |
| Omar Hassan | 2026-07 | 270.87 | 175.91 | 94.96 |
| Omar Hassan | 2026-08 | 561.90 | 353.30 | 208.60 |
| Omar Hassan | 2026-09 | 1,194.21 | 932.00 | 262.21 |
| Ana Ruiz | 2025-10 | 138.55 | 138.55 | 0.00 |
| Ana Ruiz | 2025-11 | 433.13 | 355.12 | 78.01 |
| Ana Ruiz | 2025-12 | 352.36 | 277.46 | 74.90 |
| Ana Ruiz | 2026-01 | 570.64 | 462.50 | 108.14 |
| Ana Ruiz | 2026-02 | 180.22 | 112.32 | 67.90 |
| Ana Ruiz | 2026-03 | 186.41 | 186.41 | 0.00 |
| Ana Ruiz | 2026-04 | 340.64 | 252.44 | 88.20 |
| Ana Ruiz | 2026-05 | 577.61 | 527.02 | 50.59 |
| Ana Ruiz | 2026-06 | 275.15 | 254.85 | 20.30 |
| Ana Ruiz | 2026-07 | 192.23 | 106.38 | 85.85 |
| Ana Ruiz | 2026-08 | 171.01 | 171.01 | 0.00 |
| Ana Ruiz | 2026-09 | 289.25 | 289.25 | 0.00 |
| Grace Okafor | 2025-10 | 82.60 | 82.60 | 0.00 |
| Grace Okafor | 2025-11 | 57.06 | 57.06 | 0.00 |
| Grace Okafor | 2025-12 | 41.30 | 41.30 | 0.00 |
| Grace Okafor | 2026-01 | 41.30 | 41.30 | 0.00 |
| Grace Okafor | 2026-02 | 30.80 | 0.00 | 30.80 |
| Grace Okafor | 2026-04 | 159.01 | 76.41 | 82.60 |
| Grace Okafor | 2026-05 | 132.16 | 132.16 | 0.00 |
| Grace Okafor | 2026-06 | 109.45 | 109.45 | 0.00 |
| Grace Okafor | 2026-07 | 356.70 | 308.40 | 48.30 |
| Grace Okafor | 2026-08 | 234.26 | 234.26 | 0.00 |
| Grace Okafor | 2026-09 | 449.39 | 449.39 | 0.00 |
| Daniel Fischer | 2025-10 | 29.16 | 29.16 | 0.00 |
| Daniel Fischer | 2025-11 | 239.44 | 239.44 | 0.00 |
| Daniel Fischer | 2025-12 | 29.16 | 29.16 | 0.00 |
| Daniel Fischer | 2026-03 | 68.60 | 68.60 | 0.00 |
| Daniel Fischer | 2026-04 | 92.61 | 92.61 | 0.00 |
| Daniel Fischer | 2026-05 | 306.99 | 257.25 | 49.74 |
| Daniel Fischer | 2026-06 | 34.30 | 0.00 | 34.30 |
| Daniel Fischer | 2026-08 | 57.93 | 57.93 | 0.00 |
| Daniel Fischer | 2026-09 | 61.60 | 61.60 | 0.00 |
| Mei Lin | 2025-11 | 211.82 | 158.87 | 52.95 |
| Mei Lin | 2025-12 | 118.37 | 62.30 | 56.07 |
| Mei Lin | 2026-01 | 124.60 | 124.60 | 0.00 |
| Mei Lin | 2026-02 | 124.60 | 124.60 | 0.00 |
| Mei Lin | 2026-03 | 62.30 | 62.30 | 0.00 |
| Mei Lin | 2026-04 | 221.17 | 221.17 | 0.00 |
| Mei Lin | 2026-05 | 348.88 | 348.88 | 0.00 |
| Mei Lin | 2026-06 | 261.35 | 261.35 | 0.00 |
| Mei Lin | 2026-07 | 95.32 | 95.32 | 0.00 |
| Mei Lin | 2026-08 | 105.91 | 105.91 | 0.00 |
| Mei Lin | 2026-09 | 52.96 | 52.96 | 0.00 |
| Carlos Mendes | 2025-10 | 108.47 | 89.90 | 18.57 |
| Carlos Mendes | 2025-11 | 401.17 | 361.45 | 39.72 |
| Carlos Mendes | 2025-12 | 196.06 | 171.49 | 24.57 |
| Carlos Mendes | 2026-01 | 491.10 | 463.80 | 27.30 |
| Carlos Mendes | 2026-02 | 127.61 | 77.81 | 49.80 |
| Carlos Mendes | 2026-03 | 311.31 | 284.01 | 27.30 |
| Carlos Mendes | 2026-04 | 183.77 | 170.47 | 13.30 |
| Carlos Mendes | 2026-05 | 335.68 | 284.79 | 50.89 |
| Carlos Mendes | 2026-06 | 181.64 | 157.07 | 24.57 |
| Carlos Mendes | 2026-07 | 61.81 | 50.51 | 11.30 |
| Carlos Mendes | 2026-08 | 189.09 | 175.79 | 13.30 |
| Carlos Mendes | 2026-09 | 40.60 | 40.60 | 0.00 |
| Hannah Cohen | 2025-10 | 129.58 | 129.58 | 0.00 |
| Hannah Cohen | 2025-11 | 380.14 | 309.93 | 70.21 |
| Hannah Cohen | 2025-12 | 443.16 | 319.26 | 123.90 |
| Hannah Cohen | 2026-01 | 473.96 | 364.47 | 109.49 |
| Hannah Cohen | 2026-02 | 370.73 | 329.43 | 41.30 |
| Hannah Cohen | 2026-03 | 306.28 | 271.17 | 35.11 |
| Hannah Cohen | 2026-04 | 266.60 | 253.30 | 13.30 |
| Hannah Cohen | 2026-05 | 695.85 | 596.60 | 99.25 |
| Hannah Cohen | 2026-06 | 328.55 | 287.25 | 41.30 |
| Hannah Cohen | 2026-07 | 426.15 | 398.85 | 27.30 |
| Hannah Cohen | 2026-08 | 342.20 | 342.20 | 0.00 |
| Hannah Cohen | 2026-09 | 269.89 | 228.59 | 41.30 |
| **Total (139 instructor-months)** | | **61,663.66** | **54,024.62** | **7,639.04** |

## Extra note: 1 October 2026 fixtures (not in the total)

Eight order lines are dated 2026-10-01 12:00:00, outside the scope window. They are the dataset's "today" fixtures: orders 1201, 1202, 1204, 1205, 1207, 1208, 1210 and 1211, all for course 6 (Deploying Apps with Containers, 60.00 each). Four were refunded and four were paid.

Course 6 belongs to **Omar Hassan** (instructor 6). For 2026-10, the reported share would be 70% of 480.00 = 336.00, against a correct share of 70% of 240.00 = 168.00. That is over-paid 168.00. This is informational only and is **not** included in the 7,639.04 total.

## Data integrity

| Check | Result |
| --- | --- |
| Refunded orders with no refunds row | 0 |
| Paid orders with a refunds row | 0 |
| `refunds.amount` differs from the order's line total | 0 cases (equal in every case) |
| `orders.total` differs from the line sum | 0 cases (equal in every case) |
| Instructor-months with lines but no paid lines | 2 (Grace Okafor 2026-02, Daniel Fischer 2026-06) |
| Statuses other than Paid or Refunded | 0 |

- **Refund timing.** 25 refunds were made in a later month than the sale. Each refund is deducted from the month of the sale. This is an assumption, the same basis as the instructor revenue figures and the MCP `instructor_payout` tool. A refund-month basis would move amounts between months but would not change the grand total of refunded money.
- **Month boundary.** Order 252 sits within an hour of a month boundary. Its month is taken from `orders.created_at` as stored.

## Cause

`PayoutReportBuilder.Build` summed every order line created in the month and did not filter on `orders.status`. Lines from Refunded orders were therefore included, and instructors were paid 70% of refunded money.

The revenue page (`ShopService.GetInstructorRevenue`) and the MCP tool already netted refunds, which is why they disagreed with the payout report. Refunds are whole-order only, so every line in a refunded order is refunded.

## Fix and tests

- **Fix: commit `dfa2c3c`.** A `status = 'Paid'` filter in `backend/LearnHub.Api/Reports/PayoutReportBuilder.cs`, with a one-line comment.
- **Tests: commit `6eb2916`.** Four new tests in `backend/LearnHub.Tests/PayoutReportTests.cs`:
  - a refunded order is excluded from the instructor row and the totals;
  - an instructor with only refunded orders has no row, and other rows are unchanged;
  - an order bought in March and refunded in April is excluded from the March report;
  - the report total share equals the sum of the instructor revenue shares.
- **Status.** These tests failed before the fix, and the full backend suite (104 tests) passes after it.

## Recommendation

- **Finance should recover or offset the 7,639.04 over-payments** listed in the per-instructor table. The choice between recovery and offset against future payouts is a finance decision.
- **Check the 2026-10 figures after the fix.** The report window ends at 2026-09, so the fixtures above (Omar Hassan, 168.00) should be checked when October is run.

## Remaining limits

- **Historical payouts have not been reversed or recovered.** The 7,639.04 is the amount to be recovered or offset.
- **Refund-month basis.** The 25 later-month refunds are deducted from the sale month. A different basis would change the monthly split, not the grand total.
- **Scope.** The figures are from read-only SELECTs against the `learnhub` database. The four new tests were run before and after the fix (4 failed, then all passed), and the full backend suite passed (104 tests).

## Appendix: Method

- **Population:** order lines on orders created 2025-10-01 to 2026-09-30, grouped by instructor (via the course) and by `DATE_FORMAT(orders.created_at, '%Y-%m')`.
- **Shares:** `ROUND(gross * 0.7, 2)`. Reported uses all lines. Correct uses only lines where `orders.status = 'Paid'`.
- **Difference:** reported share minus correct share. Positive means over-paid.
- **Source:** read-only SELECTs against the `learnhub` database. The row count (139) and the three totals were checked with a second, independent query.
