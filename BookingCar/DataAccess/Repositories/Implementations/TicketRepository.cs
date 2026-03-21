using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
{
    public TicketRepository(AppDbContext context) : base(context)
    {
    }

    public Task<List<Ticket>> GetByScheduleIdAsync(int scheduleId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Ticket>> GetByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Ticket?> GetTicketWithUserByIdAsync(int ticketId)
    {
        return await _dbSet
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == ticketId);
    }
}