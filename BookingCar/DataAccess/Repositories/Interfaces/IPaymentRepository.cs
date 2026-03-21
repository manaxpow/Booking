public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment?> GetByTicketPendingByIdAsync(int ticketId);
}