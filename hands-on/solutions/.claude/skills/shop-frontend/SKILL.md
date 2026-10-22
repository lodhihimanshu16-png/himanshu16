---
name: shop-frontend
description: Builds the shop screens of the LearnHub React frontend - price and Add to cart on course cards, Cart page with coupon box and live price breakdown, My Orders page with Refund button, Instructor Revenue page, nav links. Use after the shop backend API exists and the task is about the shop screens. See the Screens list in FEATURE.md.
---

# Shop frontend

Work in `frontend/src`. Read the "Screens" and "API" parts of `FEATURE.md` and `.claude/skills/add-react-page/SKILL.md` first. Plain JavaScript. Copy the style of `pages/MyCoursesPage.jsx`.
Do not touch `backend/`. Do not add packages.

## Steps

1. Add to `api.js`, all with `request()`:
   - `formatPrice(price)`: `$40.00`, or `Free` when the price is 0.
   - `getCart`, `addToCart`, `removeFromCart`, `previewCheckout`, `checkout`, `getOrders`, `refundOrder`, `getInstructorRevenue`.
2. `components/CourseCard.jsx`: show the price (or "Free"). Paid courses show an "Add to cart" button when an `onAddToCart` prop is given. The card is a link, so call `e.preventDefault()` in the click handler.
3. `pages/CoursesPage.jsx`: pass `onAddToCart`. It uses the saved student name (`getSavedName()`). Show success with `<p className="message ok">` and errors with `<p className="message error">`.
4. `pages/CourseDetailPage.jsx`: show the price. For a paid course the button says "Add to cart" and calls `addToCart` instead of `enroll`.
5. `pages/CartPage.jsx` (route `/cart`):
   - Name box and coupon box. Load the cart, then call `previewCheckout` to show each line (price, bundle, coupon, final) and the totals. Show `couponMessage`. Do not do the price maths in the page. The backend does it.
   - A Remove button per line and a "Pay" button that calls `checkout`. After paying, show the order number and reload.
6. `pages/MyOrdersPage.jsx` (route `/orders`): list the orders with date, status, total and lines. A Paid order has a Refund button. When the refund is refused, show the reason under that order.
7. `pages/InstructorRevenuePage.jsx` (route `/instructor-revenue`): a table of instructor, gross sales, refunds, instructor share.
8. Routes in `App.jsx`. Nav links in `components/NavBar.jsx`: Cart, My Orders, Revenue.
9. Styles: reuse `panel`, `row`, `muted`, `message`. Add a few new classes to the end of `index.css` only if needed (price, table).
10. Build: `cd frontend && npm install && npm run build`. Fix errors.
11. Tell the user the new URLs and which files changed.
