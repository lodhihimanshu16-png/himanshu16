---
name: shop-backend
description: Builds the shopping part of the LearnHub backend - course prices, the cart, the 15% bundle discount and coupons (models, InMemoryStore methods, endpoints). Use when the task is about prices, a cart, bundles or coupons. Rules R1 to R14 in FEATURE.md.
---

# Shop backend: prices, cart, bundle, coupons

Work in `backend/LearnHub.Api`. Read `FEATURE.md` rules R1 to R14 first. Copy the style of `Endpoints/EnrollmentEndpoints.cs` and `Data/InMemoryStore.cs`.
This skill does NOT build checkout, orders, refunds or revenue. The `shop-orders` skill does that.

## Steps

1. Prices (R1).
   - Add `public decimal Price { get; set; }` to `Models/Course.cs`. 0 means free.
   - Set `Price` on each course in `InMemoryStore`: course 1 = 0, 2 = 40, 3 = 30, 4 = 30, 5 = 0, 6 = 60 (see "Starting data" in FEATURE.md).
   - Add `c.Price` and `course.Price` to both anonymous objects in `Endpoints/CourseEndpoints.cs`.
2. Models. One file each in `Models/`, plain classes with defaults.
   - `CartItem.cs`: `StudentName`, `CourseId`, `AddedAt`.
   - `Coupon.cs`: `Code`, `Type` ("Percent" or "Fixed"), `Value`, `ExpiresOn`, `MaxUses`, `UsedCount`, `MinOrderAmount`, `Category` (empty means every course).
   - `Order.cs`: classes `OrderLine` (CourseId, Title, Instructor, Price, BundleDiscount, CouponDiscount, Final), `Order` (Id, StudentName, CreatedAt, Lines, CouponCode, Subtotal, BundleDiscount, CouponDiscount, Total, Status) and `PriceQuote` (Lines, Subtotal, BundleDiscount, CouponDiscount, Total, CouponMessage, CouponRejected, CouponCode).
3. Store data in `Data/InMemoryStore.cs`.
   - A clock the tests can replace: `public Func<DateTime> Clock { get; set; } = () => DateTime.UtcNow;`. Always use `Clock()`, never `DateTime.UtcNow`, for shop dates.
   - `List<CartItem> Cart`, `List<Order> Orders`, and `List<Coupon> Coupons` with the five coupons from FEATURE.md.
   - Enrollment ids: replace `Enrollments.Count + 1` with a counter `_nextEnrollmentId++`. Refunds will remove enrollments later, and ids must never be reused.
4. Store methods (logic goes here, not in endpoints).
   - `EnrollFree(courseId, studentName)`: paid course gives an error (R2), otherwise calls `Enroll`. Use it in `Endpoints/EnrollmentEndpoints.cs`. Keep `Enroll` as it is, because checkout uses it.
   - `AddToCart`, `RemoveFromCart`, `IsInCart`, `GetCart` (R3 to R5). Errors are text, like `Enroll`.
   - `Quote(studentName, couponCode)` returns a `PriceQuote` and changes nothing (R6, R10, R13, R14). Order of maths: bundle first, then coupon.
   - A private `ApplyCoupon` that checks R7 to R12 and fills `CouponDiscount` on the lines.
   - `HasUsedCoupon(studentName, code)`: true when a Paid order by that student used the code (R9).
   - A private `Money(decimal)` that does `Math.Round(value, 2, MidpointRounding.AwayFromZero)` (R13).
   - Fixed coupons: the discount is at most the matching lines' price. Share it between the lines, and give the last line the leftover so the parts add up (R12).
5. Endpoints (R3 to R5, R14). Follow CLAUDE.md error rules.
   - `Endpoints/CartEndpoints.cs`: `record CartRequest(string? StudentName, int CourseId)`; `GET /api/cart`, `POST /api/cart` (201, 400, 409), `DELETE /api/cart` (204 or 404).
   - `Endpoints/CheckoutEndpoints.cs`: `record CheckoutRequest(string? StudentName, string? CouponCode)`; `POST /api/checkout/preview` returns `lines` (courseId, title, price, bundleDiscount, couponDiscount, final), `subtotal`, `bundleDiscount`, `couponDiscount`, `total`, `couponMessage`. Leave the `POST /api/checkout` route to `shop-orders`.
6. Register `app.MapCartEndpoints();` and `app.MapCheckoutEndpoints();` in `Program.cs`.
7. Build: `cd backend && dotnet build`. Fix errors until there are 0. Run `dotnet test`. The old tests must still pass.
8. Tell the user which files you added or changed and which FEATURE.md rules are done. Do not touch the frontend or write tests unless asked.
