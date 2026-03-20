using DataAccess.Data;

public class TicketDetailRepository : GenericRepository<TicketDetail>, ITicketDetailRepository
{
    public TicketDetailRepository(AppDbContext context) : base(context)
    {
    }
}