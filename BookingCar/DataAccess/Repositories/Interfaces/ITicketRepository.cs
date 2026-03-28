public interface ITicketRepository : IGenericRepository<Ticket>
{
    Task<List<Ticket>> GetByUserIdAsync(int userId);
    Task<Ticket?> GetTicketWithUserByIdAsync(int ticketId);
}