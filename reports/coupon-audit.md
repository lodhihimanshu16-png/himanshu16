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

## Appendix: Method

- **Population:** orders with `status = 'Paid'`, compared case-sensitively. Refunded orders are excluded, so they do not count as a use. Refunded coupon orders (14 codes) were checked separately and do not change any result.
- **Code matching:** `UPPER(TRIM(...))` on both `orders.coupon_code` and `coupons.code`. A check found no case or whitespace variants, and every order code matches a coupon.
- **Over-limit test:** paid uses per coupon compared with `coupons.max_uses`.
- **Excess uses:** each paid order is ranked within its coupon by `created_at`, then `id`. Uses ranked above `max_uses` are excess, and their `orders.coupon_discount` values are summed.
- **Repeat use:** paid orders grouped by student and coupon. Groups with more than one order are listed.
- **Cross-checks:** (1) excess amounts recomputed by skipping the first `max_uses` paid orders (offset method), giving 29.40 and 41.66, the same as the ranking method. (2) Each excess order's `coupon_discount` equals the sum of its `order_lines.coupon_discount`. (3) The MCP tools agree with the paid-order counts for all coupons.

<!-- Root cause, fix, tests and limits: added in a later phase -->
