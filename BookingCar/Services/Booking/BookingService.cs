using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly SePaySettings _sePaySettings;
    public BookingService(IUnitOfWork unitOfWork, IOptions<SePaySettings> sePaySettings, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _sePaySettings = sePaySettings.Value;
        _emailService = emailService;
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
                if (seatBooking == null || seatBooking.IsHold)
                    throw new Exception($"Ghế {id} không tìm thấy hoặc đang có người giữ.");
                if (seatBooking.IsBooking)
                    throw new Exception($"Ghế {id} đã có người đặt nhanh hơn bạn!");

                seatBooking.IsHold = true;
                selectedSeats.Add(seatBooking);
            }
            var ticket = new Ticket
            {
                UserId = request.UserId,
                Status = TicketStatus.PENDING,
                TotalPrice = selectedSeats.Sum(x => x.Schedule.Price),
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            Console.WriteLine($"[Total price]: {ticket.TotalPrice}");

            await _unitOfWork.Tickets.AddAsync(ticket);
            await _unitOfWork.CompleteAsync();

            foreach (var seat in selectedSeats)
            {
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
                ExpiredAt = DateTime.UtcNow.AddMinutes(10),
                Amount = ticket.TotalPrice,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            // throw new Exception("TESTING ROLLBACK: Lỗi giả lập để kiểm tra giao dịch");
            await _unitOfWork.CommitTransactionAsync();

            return new BookingResponse
            {
                TicketId = ticket.Id,
                OrderCode = payment.OrderCode,
                QrUrl = GetPaymentQr(payment.Amount, payment.OrderCode),
                SeatNames = selectedSeats.Select(x => x.Seat.Name).ToList(),
                TotalAmount = payment.Amount
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL ERROR]: {ex.Message}");
            Console.WriteLine($"[STACK TRACE]: {ex.StackTrace}");

            try
            {
                // Kiểm tra an toàn trước khi gọi Rollback
                if (_unitOfWork != null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _unitOfWork.Context.ChangeTracker.Clear();
                }
            }
            catch (Exception rollbackEx)
            {
                Console.WriteLine($"[ROLLBACK ERROR]: {rollbackEx.Message}");
            }

            throw;
        }
    }

    public async Task<bool> ConfirmPaymentAsync(string transactionContent, decimal amount)
    {
        var match = Regex.Match(transactionContent, @"(?i)VE\s*(\d+)");
        if (!match.Success) return false;

        int ticketId = int.Parse(match.Groups[1].Value);

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var payment = await _unitOfWork.Payments.GetByTicketPendingByIdAsync(ticketId);

            if (payment == null)
                throw new KeyNotFoundException("Không tìm thấy giao dịch.");

            if (amount < payment.Amount)
                throw new Exception("Gia tri thanh toan khong hop le.");

            if (payment.ExpiredAt < DateTime.UtcNow)
                throw new Exception("Giao dich da het han.");

            payment.Status = PaymentStatus.PAID;

            var ticket = await _unitOfWork.Tickets.GetTicketWithUserByIdAsync(ticketId);

            if (ticket == null || ticket.Status != TicketStatus.PENDING)
                return false;

            ticket.Status = TicketStatus.CONFIRMED;
            ticket.UpdateAt = DateTime.UtcNow;

            var details = await _unitOfWork.TicketDetails.GetTicketDetailByTicketIdAsync(ticketId);
            if (details == null) return false;
            foreach (var detail in details)
            {
                detail.SeatBooking.IsBooking = true;
                detail.SeatBooking.IsHold = false;
            }

            string subject = $"[Xác nhận] Thanh toán thành công vé xe - Mã đơn {ticket.Id}";
            string body = $@"
            <div style='font-family: Arial, sans-serif; border: 1px solid #ddd; padding: 20px; max-width: 600px;'>
                <h2 style='color: #2e7d32;'>Thanh toán thành công!</h2>
                <p>Chào bạn, cảm ơn bạn đã tin tưởng dịch vụ của chúng tôi.</p>
                <hr>
                <p><strong>Mã vé:</strong> <span style='font-size: 1.2em; color: #d32f2f;'>VE{ticket.Id}</span></p>
                <p><strong>Số lượng ghế:</strong> {details.Count()}</p>
                <p><strong>Tổng tiền:</strong> {amount:N0} VNĐ</p>
                <p><strong>Trạng thái:</strong> Đã thanh toán qua SePay</p>
                <hr>
                <p style='font-size: 0.9em; color: #666;'>Vui lòng trình mã vé này cho phụ xe khi lên xe.</p>
            </div>";

            await _emailService.SendEmailAsync(ticket.User.Email, subject, body);

            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();

            Console.WriteLine($"[FATAL ERROR]: {ex.Message}");
            Console.WriteLine($"[STACK TRACE]: {ex.StackTrace}");
            return false;
        }
    }


    public async Task<PaymentInfoResponse> GetPaymentDetailsAsync(int paymentId)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);

        if (payment == null)
            throw new KeyNotFoundException("Không tìm thấy giao dịch.");

        // 3. Trả về DTO (Không cần dùng Select)
        return new PaymentInfoResponse
        {
            OrderCode = payment.OrderCode,
            Amount = payment.Amount,
            Status = payment.Status,
            ExpiresAt = payment.ExpiredAt ?? DateTime.UtcNow
        };
    }

    public string GetPaymentQr(decimal amount, string orderCode)
    {
        return SePayHelper.GenerateQrUrl(
            _sePaySettings.AccountNumber,
            _sePaySettings.BankName,
            amount,
            orderCode
        );
    }
}