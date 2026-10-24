# Refund policy audit

Prepared for: Finance Director
Scope: all 150 refunds in the LearnHub dataset, judged as at 1 October 2026 (the dataset's "today").

## Summary

- **9 of 150 refunds (6%) broke the refund policy.**
- **Total refunded in breach: 544.16.** Every one of the nine refunded the full order value, so the exposure is the full amount, not a partial refund.
- **6 breached the time limit** (refunded more than 7 days after purchase). **3 breached the progress limit** (the student had completed 30% or more of a course in the order). No refund breached both.
- **FLASH40 coupon:** 5 of the 9 refunds used it, worth 299.16. The other 4 had no coupon, worth 245.00.
- Refund money was concentrated in June 2026 (263.76, four refunds), with 177.00 in December 2025 (two refunds).

The policy requires that a refund is allowed only if both of these hold:

1. It is requested within 7 days of purchase, and
2. Progress is below 30% in every course in the order.

## Money by month of refund

| Month | Refunds | Amount |
|---|---|---|
| December 2025 | 32, 41 | 177.00 |
| January 2026 | 64 | 19.00 |
| May 2026 | 112 | 35.40 |
| June 2026 | 119, 121, 122, 125 | 263.76 |
| August 2026 | 141 | 49.00 |
| **Total** | **9** | **544.16** |

## Refunds in breach

| Refund | Order | Student | Course(s) | Refunded | Coupon | Breach |
|---|---|---|---|---|---|---|
| 32 | 247 | Sara Ali | TypeScript in Depth (83%); Building REST APIs with .NET (67%) | 128.00 | None | Progress |
| 41 | 290 | Rhea Kowalski | React Fundamentals | 49.00 | None | Time (22.7 days after purchase) |
| 64 | 435 | Kavya Fernandez | Business Writing (86%) | 19.00 | None | Progress |
| 112 | 838 | Victor Abdi | Serverless Applications (67%) | 35.40 | FLASH40 | Progress |
| 119 | 778 | Yara Cohen | Python for Data Analysis; Motion and Interaction Design | 64.80 | FLASH40 | Time (24.8 days) |
| 121 | 867 | Wen Tanaka | Object-Oriented Design in C#; Project Management Basics; React Fundamentals; Finance for Non-Finance Managers | 110.16 | FLASH40 | Time (11.3 days) |
| 122 | 762 | Quinn Reyes | Design Systems | 41.40 | FLASH40 | Time (28.5 days) |
| 125 | 908 | Rhea Yilmaz | Cloud Networking; Data Analysis with SQL | 47.40 | FLASH40 | Time (17.2 days) |
| 141 | 1025 | Olga Garcia | Cloud Networking | 49.00 | None | Time (26.3 days) |

Percentages are the student's progress in that course at the time of refund. Refund 121 was in time-limit breach only, with no recorded progress. The same breaches are reported by the LearnHub MCP tool `find_refund_policy_violations`.

## Root cause

The refund logic lives in `ShopService.RefundOrder`. Three weaknesses let these refunds through.

1. **The 7-day clock starts from the wrong date.** The check measures from `Order.UpdatedAt`, which can change after purchase, rather than from the original purchase date (`CreatedAt`). In the data, 95 paid orders already have an updated date that differs from the created date, so the window can be moved.
2. **The rules exist in one place only, and the database does not enforce them.** The refunds table has no constraint. Any other route into the database, such as a script, an import or a manual SQL change, bypasses the check entirely.
3. **Progress is a live count taken at the moment of refund.** The audit calculates progress separately, so the service and the audit can disagree about the same refund.

**Caveat, stated plainly.** The current service code would have rejected all nine refunds had they gone through it. For the later refunds, the order's updated date equals the refund date, so the service's date logic cannot reproduce them either. The most likely explanations are that these refunds bypassed the service, or that they were made by an earlier version of the code. We have not confirmed which.

## Fix

To be completed after the code fix and tests land.

## Tests

To be completed after the code fix and tests land.

## Follow-up (not applied)

A database trigger that rejects refunds breaking the policy has been considered but deliberately not applied. The seed file `db/seed.sql` contains these nine historical violations, so a trigger would cause the seed to fail on load. Any trigger should be added only after the seed data is corrected.

## Appendix: Method

The audit joins each refund to its order and order lines, then tests each refund against both limits.

- **Time limit:** `TIMESTAMPDIFF` in seconds (or days, with fractions) between `orders.created_at` and `refunds.refunded_at`. Breach if the result is greater than 7 days.
- **Progress limit:** for each course in the order, count lessons with `lesson_progress.completed_at <= refunds.refunded_at`, divide by the total lessons in that course, and express as a percentage. Breach if any course is 30% or more.
- **Totals:** sum the refund amounts for breaching refunds, grouped by refund month and by coupon code.
- The exact timestamps and per-course progress were checked for each of the nine refunds.
