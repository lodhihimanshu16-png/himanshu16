---
name: shop-orders
description: Builds checkout, orders, refunds and instructor revenue in the LearnHub backend (store methods and endpoints). Use after the cart and coupons exist (shop-backend) and the task is about paying, orders, refunds or what instructors earn. Rules R15 to R20 in FEATURE.md.
---

# Shop orders: checkout, refunds, revenue

Work in `backend/LearnHub.Api`. Read `FEATURE.md` rules R15 to R20 first.
This skill needs `Quote`, `Cart`, `Orders`, `Coupons` and `Clock` in `Data/InMemoryStore.cs`. If they are missing, stop and use the `shop-backend` skill first.

## Steps

1. Checkout in `InMemoryStore` (R15, R16).
   - `Checkout(studentName, couponCode)` returns `(Order? Order, string? Error)`.
   - Check in this order: name is given, cart is not empty ("Your cart is empty."), the student does not already own any course in the cart, then call `Quote`. If `CouponRejected`, return its `CouponMessage` as the error.
   - Then save the order: `Id` from a counter `_nextOrderId++`, `CreatedAt = Clock()`, lines and totals copied from the quote, `Status = "Paid"`.
   - Add 1 to the coupon's `UsedCount`. Call `Enroll` for every line. Remove the student's cart items.
2. Orders in `InMemoryStore` (R17). `GetOrders(studentName)` returns the student's orders, newest first.
3. Refunds in `InMemoryStore` (R18, R19). `RefundOrder(orderId)` returns `(Order? Order, string? Error)`.
   - Errors, in this order: "Order not found.", "This order was already refunded.", too old, progress too high.
   - Too old: `Clock() - order.CreatedAt` is more than 7 days. Exactly 7 days is still fine.
   - Progress: for every line, the share of lessons the student finished must be under 30%. Use the real ratio, not the rounded percent. The message names the course and the percent, for example "You have finished 40% of Data Analysis with SQL. Refunds need progress under 30% on every course."
   - On success: `Status = "Refunded"`, remove the student's `Enrollments` and `Progress` for the course in each line, and subtract 1 from the coupon's `UsedCount` (never below 0).
4. Revenue in `InMemoryStore` (R20). `GetInstructorRevenue()` returns one row per instructor name found in `Courses`: `GrossSales` (all line finals), `Refunds` (line finals of Refunded orders), `InstructorShare` = 70% of (gross minus refunds), rounded with `Money`.
5. Endpoints. Follow CLAUDE.md error rules.
   - Add `POST /api/checkout` to `Endpoints/CheckoutEndpoints.cs`: 201 with the order, or 400 `{ error }`.
   - `Endpoints/OrderEndpoints.cs`: `GET /api/orders?studentName=`; `POST /api/orders/{id:int}/refund` gives 200, 400, 404 or 409 (match on the error text, like `EnrollmentEndpoints.cs`).
   - `Endpoints/RevenueEndpoints.cs`: `GET /api/instructors/revenue` returns `instructor`, `grossSales`, `refunds`, `instructorShare`.
6. Register `app.MapOrderEndpoints();` and `app.MapRevenueEndpoints();` in `Program.cs`.
7. Build and test: `cd backend && dotnet build && dotnet test`. Fix errors.
8. Optional check: start the API, try the flow with `curl`, then stop the API.
9. Tell the user which files changed and which rules (R15 to R20) are done.
