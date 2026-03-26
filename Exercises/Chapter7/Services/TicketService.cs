using Chapter6.DataAccess.UnitOfWork;
using Chapter6.DTOs.Tickets;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<GetTicketByIdResponse>> GetByIdForUserAsync(
        int id,
        int currentUserId,
        bool isAdmin
    )
    {
        if (isAdmin)
        {
            var ticketForAdmin = await _unitOfWork.SeatBookings.GetByIdAsync(id);
            if (ticketForAdmin is null)
            {
                return ServiceResult<GetTicketByIdResponse>.NotFound(
                    $"Không tìm thấy ticket id={id}."
                );
            }

            return ServiceResult<GetTicketByIdResponse>.Success(Map(ticketForAdmin));
        }

        // BOLA protection: user chỉ xem được ticket của chính mình.
        var tickets = await _unitOfWork.SeatBookings.FindAsync(t =>
            t.Id == id && t.UserId == currentUserId
        );
        var ticket = tickets.FirstOrDefault();
        if (ticket is null)
        {
            // Trả 404 thay vì 403 để tránh lộ sự tồn tại của resource.
            return ServiceResult<GetTicketByIdResponse>.NotFound($"Không tìm thấy ticket id={id}.");
        }

        return ServiceResult<GetTicketByIdResponse>.Success(Map(ticket));
    }

    private static GetTicketByIdResponse Map(Models.SeatBooking ticket) =>
        new()
        {
            Id = ticket.Id,
            ScheduleId = ticket.ScheduleId,
            SeatNumber = ticket.SeatNumber,
            IsBooking = ticket.IsBooking,
            IsHold = ticket.IsHold,
            UserId = ticket.UserId,
        };
}
