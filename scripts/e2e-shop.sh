#!/usr/bin/env bash
# End-to-end check of the paid courses flow against the running API.
# Usage: bash scripts/e2e-shop.sh [base-url]   (default http://localhost:5080)

BASE="${1:-http://localhost:5080}"
TMP=$(mktemp)
FAILS=0
CODE=""
BODY=""

# Fresh student names every run
RUN="$(date +%Y%m%d%H%M%S)$RANDOM"
S1="e2eA$RUN"
S2="e2eB$RUN"
S3="e2eC$RUN"

req() { # method path [json]
  if [ -n "$3" ]; then
    CODE=$(curl -s -o "$TMP" -w '%{http_code}' -X "$1" -H 'Content-Type: application/json' -d "$3" "$BASE$2")
  else
    CODE=$(curl -s -o "$TMP" -w '%{http_code}' -X "$1" "$BASE$2")
  fi
  BODY=$(cat "$TMP")
}

has() { grep -Eq "$1" <<<"$BODY"; }                 # regex found in last body
err() { grep -o '"error":"[^"]*"' <<<"$BODY" | head -1; }
first_id() { grep -o '"id":[0-9]*' <<<"$BODY" | head -1 | grep -o '[0-9]*$'; }

result() { # name exit-status reason
  if [ "$2" -eq 0 ]; then
    echo "PASS  $1"
  else
    echo "FAIL  $1 ($3)"
    FAILS=$((FAILS + 1))
  fi
}

liam_share() {
  req GET /api/instructors/revenue
  grep -o '"instructor":"Liam Carter"[^}]*' <<<"$BODY" | grep -o 'instructorShare":[0-9.]*' | cut -d: -f2
}

# 1. Courses show a price
req GET /api/courses/2
has '"price":40(\.0+)?[,}]'
result "courses show a price" $? "course 2 has no price 40, HTTP $CODE"

# 2. Add paid courses 2, 3, 4 to the cart
for c in 2 3 4; do
  req POST /api/cart "{\"studentName\":\"$S1\",\"courseId\":$c}"
  [ "$CODE" = 201 ]
  result "add course $c to cart" $? "HTTP $CODE: $(err)"
done

# 3. Same course twice gives 409
req POST /api/cart "{\"studentName\":\"$S1\",\"courseId\":2}"
[ "$CODE" = 409 ]
result "adding course 2 twice gives 409" $? "HTTP $CODE: $(err)"

# 4. Free course gives 400
req POST /api/cart "{\"studentName\":\"$S1\",\"courseId\":1}"
[ "$CODE" = 400 ]
result "free course 1 gives 400" $? "HTTP $CODE: $(err)"

# 5. Preview with WELCOME10 gives total 76.5
req POST /api/checkout/preview "{\"studentName\":\"$S1\",\"couponCode\":\"welcome10\"}"
[ "$CODE" = 200 ] && has '"total":76\.5(0)?[,}]'
result "preview with welcome10 gives total 76.5" $? "HTTP $CODE, body: $BODY"

# 6. Expired coupon is explained in couponMessage
req POST /api/checkout/preview "{\"studentName\":\"$S1\",\"couponCode\":\"OLD5\"}"
[ "$CODE" = 200 ] && has '"couponMessage":"[^"]+"'
result "expired coupon OLD5 explained in couponMessage" $? "HTTP $CODE, body: $BODY"

# 7. Checkout with an empty cart (fresh student) gives 400
req POST /api/checkout "{\"studentName\":\"$S2\"}"
[ "$CODE" = 400 ]
result "checkout with empty cart gives 400" $? "HTTP $CODE: $(err)"

# 8. Checkout gives 201. Capture instructor revenue before it.
REV_BEFORE=$(liam_share)
req POST /api/checkout "{\"studentName\":\"$S1\",\"couponCode\":\"welcome10\"}"
[ "$CODE" = 201 ]
result "checkout gives 201" $? "HTTP $CODE: $(err)"
ORDER_ID=$(first_id)

# 9. Cart is empty afterwards
req GET "/api/cart?studentName=$S1"
[ "$CODE" = 200 ] && has '^\[\]$'
result "cart is empty after checkout" $? "HTTP $CODE, body: $BODY"

# 10. Order is listed
req GET "/api/orders?studentName=$S1"
[ "$CODE" = 200 ] && has "\"id\":$ORDER_ID[,}]" && has '"status":"Paid"'
result "order is listed" $? "HTTP $CODE, order $ORDER_ID not found"

# 11. Student is enrolled in courses 2, 3 and 4
req GET "/api/enrollments?studentName=$S1"
[ "$CODE" = 200 ] && has '"courseId":2[,}]' && has '"courseId":3[,}]' && has '"courseId":4[,}]'
result "student is enrolled in courses 2, 3 and 4" $? "HTTP $CODE, body: $BODY"

# 12. Same coupon cannot be used twice by this student
req POST /api/cart "{\"studentName\":\"$S1\",\"courseId\":6}"
req POST /api/checkout "{\"studentName\":\"$S1\",\"couponCode\":\"welcome10\"}"
[ "$CODE" = 400 ]
result "welcome10 cannot be used twice by the same student" $? "HTTP $CODE: $(err)"

# 13. Instructor revenue for Liam Carter goes up by 21.42
REV_AFTER=$(liam_share)
awk -v a="$REV_AFTER" -v b="$REV_BEFORE" 'BEGIN { d = a - b; exit !(d > 21.415 && d < 21.425) }'
result "Liam Carter revenue goes up by 21.42" $? "before ${REV_BEFORE:-none}, after ${REV_AFTER:-none}"

# 14. Refund gives 200
req POST "/api/orders/$ORDER_ID/refund"
[ "$CODE" = 200 ]
result "refund gives 200" $? "HTTP $CODE: $(err)"

# 15. Enrollments are gone after refund
req GET "/api/enrollments?studentName=$S1"
[ "$CODE" = 200 ] && has '^\[\]$'
result "enrollments are gone after refund" $? "HTTP $CODE, body: $BODY"

# 16. Second refund gives 409
req POST "/api/orders/$ORDER_ID/refund"
[ "$CODE" = 409 ]
result "second refund gives 409" $? "HTTP $CODE: $(err)"

# 17. Refund at 40% progress gives 400
req POST /api/cart "{\"studentName\":\"$S3\",\"courseId\":4}"
req POST /api/checkout "{\"studentName\":\"$S3\"}"
[ "$CODE" = 201 ]
result "fresh student buys course 4" $? "HTTP $CODE: $(err)"
ORDER3=$(first_id)

req POST /api/progress "{\"studentName\":\"$S3\",\"courseId\":4,\"lessonId\":12}"
P1=$CODE
req POST /api/progress "{\"studentName\":\"$S3\",\"courseId\":4,\"lessonId\":13}"
[ "$P1" = 200 ] && [ "$CODE" = 200 ]
result "complete 2 of 5 lessons in course 4" $? "HTTP $P1 / $CODE: $(err)"

req POST "/api/orders/$ORDER3/refund"
[ "$CODE" = 400 ]
result "refund at 40% progress gives 400" $? "HTTP $CODE: $(err)"

echo
if [ "$FAILS" -gt 0 ]; then
  echo "$FAILS step(s) failed."
  rm -f "$TMP"
  exit 1
fi
rm -f "$TMP"
echo "All steps passed."
exit 0
