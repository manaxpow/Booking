using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

public class TicketDetailRepository : GenericRepository<TicketDetail>, ITicketDetailRepository
{
    public TicketDetailRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TicketDetail>> GetTicketDetailByTicketIdAsync(int ticketId)
    {
        return await _dbSet.Where(x => x.TicketId == ticketId).ToListAsync();
    }
}