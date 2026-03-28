using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

public class TicketDetailRepository : GenericRepository<TicketDetail>, ITicketDetailRepository
{
    public TicketDetailRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TicketDetail>> GetTicketDetailByTicketIdAsync(int ticketId)
    {
        return await _dbSet.Where(x => x.TicketId == ticketId)
                            .Include(x => x.SeatBooking)
                            .Include(x => x.SeatBooking.Schedule)
                            .Include(x => x.SeatBooking.Seat)
                            .Include(x => x.SeatBooking.Schedule.Car)
                            .Include(x => x.SeatBooking.Schedule.Driver)
                            .Include(x => x.SeatBooking.Schedule.FromDestination)
                            .Include(x => x.SeatBooking.Schedule.ToDestination)
                            .ToListAsync();
    }
}