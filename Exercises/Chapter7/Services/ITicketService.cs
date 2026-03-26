using Chapter6.DTOs.Tickets;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public interface ITicketService
{
    Task<ServiceResult<GetTicketByIdResponse>> GetByIdForUserAsync(
        int id,
        int currentUserId,
        bool isAdmin
    );
}
