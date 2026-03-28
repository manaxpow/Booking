using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
{
    public TicketRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Ticket>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<Ticket?> GetTicketWithUserByIdAsync(int ticketId)
    {
        return await _dbSet
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == ticketId);
    }
}