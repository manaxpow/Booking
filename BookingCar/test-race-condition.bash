#!/bin/bash
SEAT_ID=1
TOKEN="Your token"

echo "Bắt đầu cuộc đua đặt ghế ID: $SEAT_ID với Token..."

for i in {1..5}
do
   curl -X POST http://localhost:3000/api/Booking \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $TOKEN" \
        -d "{\"seatBookingIds\": [$SEAT_ID], \"userId\": $i}" \
        -s -o /dev/null -w "User $i: HTTP %{http_code}\n" &
done

wait
echo "Kết thúc."