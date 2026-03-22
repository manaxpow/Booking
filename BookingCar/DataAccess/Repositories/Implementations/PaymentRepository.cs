using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByTicketPendingByIdAsync(int ticketId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.TicketId == ticketId && x.Status == PaymentStatus.PENDING);
    }

    public async Task<IEnumerable<Payment>> GetExpiredPendingPaymentsAsync()
    {
        return await _dbSet
            .Where(x => x.Status == PaymentStatus.PENDING && x.ExpiredAt < DateTime.UtcNow)
            .Include(x => x.Ticket)
            .ThenInclude(x => x.TicketDetails)
            .ThenInclude(x => x.SeatBooking)
            .ToListAsync();
    }
}