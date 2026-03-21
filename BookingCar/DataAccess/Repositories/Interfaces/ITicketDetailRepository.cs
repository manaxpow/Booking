public interface ITicketDetailRepository : IGenericRepository<TicketDetail>
{
    Task<IEnumerable<TicketDetail>> GetTicketDetailByTicketIdAsync(int ticketId);
}