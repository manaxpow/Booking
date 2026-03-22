using DataAccess.Repositories.Interfaces;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore.Storage;

public interface IUnitOfWork : IDisposable
{
    // Repositories
    AppDbContext Context { get; }
    IGenericRepository<T> Repository<T>() where T : class;
    IUserRepository Users { get; }
    IDriverRepository Drivers { get; }
    ICarRepository Cars { get; }
    ISeatRepository Seats { get; }
    IDestinationRepository Destinations { get; }
    IScheduleRepository Schedules { get; }
    ITicketRepository Tickets { get; }
    IPaymentRepository Payments { get; }
    ITicketDetailRepository TicketDetails { get; }
    // Save
    Task<int> CompleteAsync();
    Task SaveChangeAsync();

    // Manage Transaction
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}