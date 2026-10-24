# Coupon audit

Prepared for: Marketing
Scope: all 1,058 paid orders and all 25 coupons in the LearnHub dataset, judged as at 1 October 2026 (the dataset's "today"). Refunded orders are not counted as a use.

## Summary

- **2 of 25 coupons were used beyond their limit:** BLACKFRI30 (48 paid uses against a limit of 47) and NEWYEAR25 (40 against 39). Each is one use over.
- **Money given away beyond the limits: 71.06** (BLACKFRI30 29.40, NEWYEAR25 41.66). Each excess use was the latest use of its coupon.
- **7 students used the same coupon more than once** (paid orders only), across REFER15, SAVE10, STUDENT15 and WELCOME10. Each repeat was exactly two uses.
- **No repeat-use student overlaps with the over-limit coupons.** The two excess orders belong to students who used their coupon once.
- **`coupons.used_count` matches the paid orders for every coupon.** No coupon has a count mismatch.
- No case or whitespace variants exist in `orders.coupon_code`, and every order code matches a coupon.
- **Cause and status:** the limit check was off by one, and the once-per-student check ran only at preview, not at checkout. Both are fixed (commits `cfa8b00` and `332db4a`).

## Coupons used beyond max_uses

| Code | max_uses | Paid uses | Uses over the limit |
|---|---|---|---|
| BLACKFRI30 | 47 | 48 | 1 |
| NEWYEAR25 | 39 | 40 | 1 |

## Money given away beyond the limits

| Coupon | Excess order ids | Excess uses | Amount given away |
|---|---|---|---|
| BLACKFRI30 | 279 | 1 | 29.40 |
| NEWYEAR25 | 472 | 1 | 41.66 |
| **Total** | | **2** | **71.06** |

Excess orders (the latest paid use of each over-limit coupon, ordered by `created_at` then `id`):

| Order | Student | Coupon | Created at | Coupon discount |
|---|---|---|---|---|
| 279 | Xavier Yilmaz (student 80) | BLACKFRI30 | 2025-12-01 23:27:17 | 29.40 |
| 472 | Tariq Ali (student 69) | NEWYEAR25 | 2026-01-10 02:07:02 | 41.66 |

## Students who used the same coupon more than once

| Student id | Student | Coupon | Paid uses | Order ids |
|---|---|---|---|---|
| 48 | Nina Bakker | REFER15 | 2 | 74, 96 |
| 17 | Pedro Mehta | SAVE10 | 2 | 66, 193 |
| 147 | Chen Jensen | SAVE10 | 2 | 19, 143 |
| 46 | Sven Ivanova | STUDENT15 | 2 | 35, 672 |
| 138 | Ben Petrov | STUDENT15 | 2 | 91, 97 |
| 162 | Rohan Ivanova | STUDENT15 | 2 | 327, 348 |
| 67 | Jamal Kowalski | WELCOME10 | 2 | 790, 845 |

Order ids are listed in order of creation.

## Coupon usage check

For all 25 coupons, `coupons.used_count` equals the number of paid orders for that code. This is true for both coupons over their limit (BLACKFRI30 48 and NEWYEAR25 40). SUMMIT50 has no orders and a count of 0. The same figures are reported by the LearnHub MCP tools `find_coupon_overuse` and `get_coupon_usage`.

## Root cause

The coupon logic lives in `ShopService.ApplyCoupon`. Two weaknesses let these uses through, and a third makes the limit fragile.

1. **Off-by-one in the limit check.** `ApplyCoupon` rejected a coupon only when `UsedCount` was greater than `MaxUses`, so a coupon already at its limit still accepted one more use. This matches the data exactly: BLACKFRI30 and NEWYEAR25 are each one over.
2. **The once-per-student check ran at preview only.** `Quote()` checked `HasUsedCoupon`, but `Checkout()` called `BuildQuote` directly and skipped that check. A student who checked out without previewing, for example by calling the API directly, could reuse a coupon. This matches the seven repeat-use students, each with two uses.
3. **The limit trusted the counter.** The limit relied on `coupons.used_count` rather than the paid orders, and the database has no constraint enforcing either rule.

## Fix

Fixed in commit `cfa8b00` on branch `bugfix/coupon-limit-enforcement`.

- **One place for the rules.** Both rules now live in `ShopService.ApplyCoupon`. Preview and checkout both reach it through `BuildQuote`, and `Quote()` simply calls `BuildQuote`.
- **Limit.** A coupon is refused when the higher of the `used_count` counter and the number of paid orders for that code (matched case-insensitively) reaches `max_uses`. Customers see "This coupon has no uses left."
- **Once per student.** A coupon is refused when the student already has a paid order with that code. Customers see "You have already used this coupon."
- **Refunds.** Refunded orders count for neither rule, so a refund gives the slot back.
- **Order of checks.** Not found, expired, no uses left, already used, minimum order, category. A coupon that is both expired and already used now reports "expired". Other messages are unchanged.

## Tests

Added in commit `332db4a`.

- **Six integration tests (R21a to R21f)**, in `backend/LearnHub.Tests/ShopTests.cs`:
  - R21a: a one-use coupon refuses the next student at preview and at checkout.
  - R21b: at the limit the coupon is refused, and one use short of it the coupon is allowed.
  - R21c: paid orders count even when the counter is wrong.
  - R21d: a refund gives the slot back.
  - R21e: checkout without a preview refuses a second use, including a lower-case code, and leaves the cart intact.
  - R21f: a refunded order does not stop the same student reusing the coupon.
- **Against the pre-fix code, five of the six fail** (R21a to R21e). R21f is a regression guard and passes either way.
- **Full backend suite:** 100 passed, 0 failed (94 before the change).

## Remaining limits

Stated plainly:

- **The database still does not enforce either rule.** A script, an import or direct SQL can still exceed a limit or repeat a coupon. We recommend a unique or guarded database rule, or restricted write access to orders, as a follow-up. This has not been applied.
- **Historical overuse is not reversed.** The 71.06 given away (BLACKFRI30 29.40, NEWYEAR25 41.66) and the seven repeat uses have not been recovered or reversed. Whether to do so is a business decision. The seed data contains them, so any database constraint would have to wait until the data is corrected.
- **Concurrency.** The check counts uses and the order is inserted afterwards, so the two steps are not atomic. Two checkouts of the last remaining use at the same instant could both pass. No transaction or lock was added.
- **Not tested end to end.** The tests use in-memory SQLite. They have not been run against MySQL or the running API.
- **MCP tools unchanged.** Only the service layer was fixed. `find_coupon_overuse` and `get_coupon_usage` already counted paid orders only, so they were not changed.

## Appendix: Method

- **Population:** orders with `status = 'Paid'`, compared case-sensitively. Refunded orders are excluded, so they do not count as a use. Refunded coupon orders (14 codes) were checked separately and do not change any result.
- **Code matching:** `UPPER(TRIM(...))` on both `orders.coupon_code` and `coupons.code`. A check found no case or whitespace variants, and every order code matches a coupon.
- **Over-limit test:** paid uses per coupon compared with `coupons.max_uses`.
- **Excess uses:** each paid order is ranked within its coupon by `created_at`, then `id`. Uses ranked above `max_uses` are excess, and their `orders.coupon_discount` values are summed.
- **Repeat use:** paid orders grouped by student and coupon. Groups with more than one order are listed.
- **Cross-checks:** (1) excess amounts recomputed by skipping the first `max_uses` paid orders (offset method), giving 29.40 and 41.66, the same as the ranking method. (2) Each excess order's `coupon_discount` equals the sum of its `order_lines.coupon_discount`. (3) The MCP tools agree with the paid-order counts for all coupons.
