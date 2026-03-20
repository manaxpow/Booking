using DataAccess.Data;

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

}