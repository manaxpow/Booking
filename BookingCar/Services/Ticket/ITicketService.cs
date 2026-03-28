using VehicleBooking.Models.DTOs.Common;
using Models.Dtos.Ticket;

namespace Services.Ticket;

public interface ITicketService
{
    Task<PagedResult<TicketResponse>> GetTicketsByUserIdAsync(int userId, int page = 1, int pageSize = 10);
    Task<TicketDetailResponse?> GetTicketByIdAsync(int ticketId);
    Task<List<TicketResponse>> GetTicketDetailsAsync(int ticketId);
}