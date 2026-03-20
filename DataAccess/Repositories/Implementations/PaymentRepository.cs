using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByTicketIdAsync(int ticketId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.TicketId == ticketId);
    }
}