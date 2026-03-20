using System.Collections;
using System.Data;
using DataAccess.Data;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _objTransaction;
    private Hashtable? _repositories;
    public IUserRepository Users { get; private set; }
    public IDriverRepository Drivers { get; private set; }
    public ICarRepository Cars { get; private set; }
    public ISeatRepository Seats { get; private set; }
    public IDestinationRepository Destinations { get; private set; }
    public IScheduleRepository Schedules { get; private set; }
    public ITicketRepository Tickets { get; private set; }
    public IPaymentRepository Payments { get; private set; }
    public ITicketDetailRepository TicketDetails { get; private set; }
    public AppDbContext Context => _context;

    public UnitOfWork(AppDbContext context)
    {

        _context = context;
        Users = new UserRepository(_context);
        Drivers = new DriverRepository(_context);
        Cars = new CarRepository(_context);
        Seats = new SeatRepository(_context);
        Destinations = new DataAccess.Repositories.Implementations.DestinationRepository(_context);
        Schedules = new DataAccess.Repositories.Implementations.ScheduleRepository(_context);
        Tickets = new TicketRepository(_context);
        Payments = new PaymentRepository(_context);
        TicketDetails = new TicketDetailRepository(_context);
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        if (_repositories == null) _repositories = new Hashtable();

        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<T>)_repositories[type]!;
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
    public async Task SaveChangeAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _objTransaction = await _context.Database.BeginTransactionAsync();
        return _objTransaction;
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_objTransaction != null)
            {
                await _objTransaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_objTransaction != null)
            {
                await _objTransaction.DisposeAsync();
                _objTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_objTransaction != null)
        {
            await _objTransaction.RollbackAsync();
            await _objTransaction.DisposeAsync();
            _objTransaction = null;
        }
    }
}