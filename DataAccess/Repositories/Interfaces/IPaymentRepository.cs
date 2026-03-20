public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment?> GetByTicketIdAsync(int ticketId);
}