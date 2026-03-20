public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    public BookingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<BookingResponse> CreateBookingAsync(SeatBookingRequest request)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null) throw new Exception("User not found.");

            var selectedSeats = new List<SeatBooking>();

            foreach (var id in request.SeatBookingIds)
            {
                var seatBooking = await _unitOfWork.Schedules.GetSeatForUpdateAsync(id);
                if (seatBooking == null || seatBooking.IsBooking)
                    throw new Exception($"Ghế {id} đã có người đặt nhanh hơn bạn!");
            }

            var ticket = new Ticket
            {
                UserId = request.UserId,
                Status = TicketStatus.PENDING,
                TotalPrice = selectedSeats.Sum(x => x.Schedule.Price),
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            await _unitOfWork.Tickets.AddAsync(ticket);
            await _unitOfWork.CompleteAsync();

            foreach (var seat in selectedSeats)
            {
                seat.IsHold = true;
                var detail = new TicketDetail
                {
                    TicketId = ticket.Id,
                    SeatBookingId = seat.Id
                };
                await _unitOfWork.TicketDetails.AddAsync(detail);
            }

            var payment = new Payment
            {
                TicketId = ticket.Id,
                OrderCode = $"VE{ticket.Id}",
                Status = PaymentStatus.PENDING,
                ExpiredAt = DateTime.UtcNow.AddMinutes(15),
                Amount = ticket.TotalPrice,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            await _unitOfWork.CommitTransactionAsync();

            return new BookingResponse
            {
                TicketId = ticket.Id,
                OrderCode = payment.OrderCode,

            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new Exception(ex.Message, ex);
        }
    }

    public Task<bool> ConfirmPaymentAsync(string transactionContent, decimal amount)
    {
        throw new NotImplementedException();
    }


    public Task<PaymentInfoResponse> GetPaymentDetailsAsync(int ticketId)
    {
        throw new NotImplementedException();
    }
}