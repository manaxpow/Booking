namespace Models.Dtos.Ticket;

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

public record TicketDetailResponse(
    int TicketId,
    int UserId,
    int SeatId,
    int ScheduleId,
    int Price,
    string From,
    string To,
    List<string> SeatsName,
    string CarName,
    string DriverName,
    DateTime DepartureTime,
    TimeSpan Duration,
    DateTime CreatedAt);