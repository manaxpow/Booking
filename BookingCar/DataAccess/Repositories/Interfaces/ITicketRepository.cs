public interface ITicketRepository : IGenericRepository<Ticket>
{
    Task<List<Ticket>> GetByScheduleIdAsync(int scheduleId);
    Task<List<Ticket>> GetByUserIdAsync(int userId);
    Task<Ticket?> GetTicketWithUserByIdAsync(int ticketId);
}