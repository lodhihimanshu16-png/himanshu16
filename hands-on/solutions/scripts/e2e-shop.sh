#!/usr/bin/env bash
# End-to-end check of the shop flow. The API must be running (default http://localhost:5080).
# Usage: bash scripts/e2e-shop.sh [base-url]
# It prints PASS or FAIL for each step and exits with a non-zero code if any step fails.

BASE="${1:-http://localhost:5080}"
STUDENT="E2E-$$-$(date +%s)"
FAILS=0
BODY=""
STATUS=""

# call METHOD PATH [JSON-BODY]  -> sets STATUS and BODY
call() {
  local out
  if [ -n "$3" ]; then
    out=$(curl -s -w $'\n%{http_code}' -X "$1" -H 'Content-Type: application/json' -d "$3" "$BASE$2")
  else
    out=$(curl -s -w $'\n%{http_code}' -X "$1" "$BASE$2")
  fi
  STATUS="${out##*$'\n'}"
  BODY="${out%$'\n'*}"
}

pass() { echo "PASS  $1"; }
fail() { echo "FAIL  $1 ($2)"; FAILS=$((FAILS + 1)); }

# expect_status "step name" 201
expect_status() {
  if [ "$STATUS" = "$2" ]; then pass "$1"; else fail "$1" "wanted HTTP $2, got $STATUS: $BODY"; fi
}

# expect_body "step name" "extended regex"
expect_body() {
  if echo "$BODY" | grep -Eq "$2"; then pass "$1"; else fail "$1" "body did not match $2: $BODY"; fi
}

# expect_not_body "step name" "extended regex"
expect_not_body() {
  if echo "$BODY" | grep -Eq "$2"; then fail "$1" "body should not match $2: $BODY"; else pass "$1"; fi
}

# revenue_of "Instructor Name" -> "gross refunds share"
revenue_of() {
  call GET /api/instructors/revenue
  echo "$BODY" | sed -n "s/.*\"instructor\":\"$1\",\"grossSales\":\([0-9.]*\),\"refunds\":\([0-9.]*\),\"instructorShare\":\([0-9.]*\).*/\1 \2 \3/p"
}

# same_number A B
same_number() { awk -v a="$1" -v b="$2" 'BEGIN { d = a - b; if (d < 0) d = -d; exit !(d < 0.005) }'; }

echo "Checking the shop flow against $BASE as student '$STUDENT'"

call GET /api/courses
if [ "$STATUS" != "200" ]; then
  echo "FAIL  The API is not answering at $BASE. Start it first: cd backend/LearnHub.Api && dotnet run"
  exit 1
fi
expect_body "1. Courses show a price" '"price":'

read -r GROSS0 REFUND0 SHARE0 <<< "$(revenue_of 'Liam Carter')"
GROSS0="${GROSS0:-0}"; REFUND0="${REFUND0:-0}"; SHARE0="${SHARE0:-0}"

# Cart
for id in 2 3 4; do
  call POST /api/cart "{\"studentName\":\"$STUDENT\",\"courseId\":$id}"
  expect_status "2. Add paid course $id to cart" 201
done
call POST /api/cart "{\"studentName\":\"$STUDENT\",\"courseId\":2}"
expect_status "3. Adding the same course twice is refused" 409
call POST /api/cart "{\"studentName\":\"$STUDENT\",\"courseId\":1}"
expect_status "4. A free course cannot go in the cart" 400
call GET "/api/cart?studentName=$STUDENT"
expect_body "5. Cart lists the three courses" '"courseId":2.*"courseId":3.*"courseId":4'

# Preview: 40 + 30 + 30 = 100, bundle -15 = 85, WELCOME10 -8.50 = 76.50
call POST /api/checkout/preview "{\"studentName\":\"$STUDENT\",\"couponCode\":\"welcome10\"}"
expect_status "6. Preview answers" 200
expect_body "7. Preview: subtotal 100, bundle 15, coupon 8.50, total 76.50" '"subtotal":100(\.0+)?,"bundleDiscount":15(\.0+)?,"couponDiscount":8\.5,?0*,"total":76\.5,?0*'
call POST /api/checkout/preview "{\"studentName\":\"$STUDENT\",\"couponCode\":\"OLD5\"}"
expect_body "8. Preview explains an expired coupon" 'expired'
call POST /api/checkout '{"studentName":"nobody-has-a-cart"}'
expect_status "9. Checkout with an empty cart is refused" 400

# Checkout
call POST /api/checkout "{\"studentName\":\"$STUDENT\",\"couponCode\":\"WELCOME10\"}"
expect_status "10. Checkout creates the order" 201
expect_body "11. Order is Paid and total is 76.50" '"total":76\.50*,"status":"Paid"'
ORDER_ID=$(echo "$BODY" | sed -n 's/.*"id":\([0-9]*\),"studentName".*/\1/p')
call GET "/api/cart?studentName=$STUDENT"
expect_body "12. Cart is empty after checkout" '^\[\]$'
call GET "/api/orders?studentName=$STUDENT"
expect_body "13. Order appears in My Orders" '"status":"Paid"'
call GET "/api/enrollments?studentName=$STUDENT"
expect_body "14. Student is enrolled in all three courses" '"courseId":2.*"courseId":3.*"courseId":4|"courseId":4.*"courseId":3.*"courseId":2'
call POST /api/cart "{\"studentName\":\"$STUDENT\",\"courseId\":2}"
expect_status "15. Owned course cannot go back in the cart" 409
call POST /api/cart "{\"studentName\":\"$STUDENT\",\"courseId\":6}"
call POST /api/checkout "{\"studentName\":\"$STUDENT\",\"couponCode\":\"WELCOME10\"}"
expect_status "16. A coupon cannot be used twice by the same student" 400
call DELETE "/api/cart?studentName=$STUDENT&courseId=6"
expect_status "17. Remove from cart works" 204

# Revenue before the refund: Liam's line for course 2 is 40 - 6 - 3.40 = 30.60
read -r GROSS1 REFUND1 SHARE1 <<< "$(revenue_of 'Liam Carter')"
if same_number "$(awk -v a="$GROSS1" -v b="$GROSS0" 'BEGIN{print a-b}')" 30.6; then pass "18. Instructor gross sales went up by 30.60"; else fail "18. Instructor gross sales" "before $GROSS0, after $GROSS1"; fi
if same_number "$(awk -v a="$SHARE1" -v b="$SHARE0" 'BEGIN{print a-b}')" 21.42; then pass "19. Instructor share went up by 21.42 (70%)"; else fail "19. Instructor share" "before $SHARE0, after $SHARE1"; fi

# Refund
call POST "/api/orders/$ORDER_ID/refund"
expect_status "20. Refund is accepted (no progress, same day)" 200
expect_body "21. Order is now Refunded" '"status":"Refunded"'
call GET "/api/enrollments?studentName=$STUDENT"
expect_body "22. Enrollments were removed" '^\[\]$'
call POST "/api/orders/$ORDER_ID/refund"
expect_status "23. A second refund is refused" 409
call POST /api/orders/99999/refund
expect_status "24. Refund of an unknown order is 404" 404

read -r GROSS2 REFUND2 SHARE2 <<< "$(revenue_of 'Liam Carter')"
if same_number "$(awk -v a="$SHARE2" -v b="$SHARE0" 'BEGIN{print a-b}')" 0; then pass "25. Instructor share is back to where it started"; else fail "25. Instructor share after refund" "before $SHARE0, after $SHARE2"; fi
if same_number "$(awk -v a="$REFUND2" -v b="$REFUND0" 'BEGIN{print a-b}')" 30.6; then pass "26. Instructor refunds went up by 30.60"; else fail "26. Instructor refunds" "before $REFUND0, after $REFUND2"; fi

# Refund refused when progress is 40% (course 4 has 5 lessons, lessons 12 and 13 done)
S2="$STUDENT-b"
call POST /api/cart "{\"studentName\":\"$S2\",\"courseId\":4}"
call POST /api/checkout "{\"studentName\":\"$S2\"}"
ORDER2=$(echo "$BODY" | sed -n 's/.*"id":\([0-9]*\),"studentName".*/\1/p')
for lesson in 12 13; do
  call POST /api/progress "{\"studentName\":\"$S2\",\"courseId\":4,\"lessonId\":$lesson}"
done
call POST "/api/orders/$ORDER2/refund"
expect_status "27. Refund refused at 40% progress" 400
expect_body "28. The reason names the progress" '40%'

echo
if [ "$FAILS" -eq 0 ]; then
  echo "All steps passed."
  exit 0
fi
echo "$FAILS step(s) failed."
exit 1
