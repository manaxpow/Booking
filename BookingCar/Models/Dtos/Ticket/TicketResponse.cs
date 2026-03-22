public record TicketResponse(
    int Id,
    int UserId,
    int SeatId,
    int ScheduleId,
    int Price,
    string From,
    string To,
    string SeatName,
    string CarName,
    string DriverName,
    DateTime DepartureTime,
    TimeSpan Duration,
    DateTime CreatedAt);