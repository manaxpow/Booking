public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(SeatBookingRequest request);
    Task<bool> ConfirmPaymentAsync(string transactionContent, decimal amount);
    Task<PaymentInfoResponse> GetPaymentDetailsAsync(int ticketId);
}