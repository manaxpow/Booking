public record SeatBookingResponse(
    int Id,
    int SeatId,
    string SeatName,
    int ScheduleId,
    bool IsHold,
    bool IsBooking,
    DateTime CreateAt,
    DateTime UpdateAt);