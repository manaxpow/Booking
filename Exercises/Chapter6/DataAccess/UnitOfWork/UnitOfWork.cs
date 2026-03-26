using Chapter6.DataAccess.Context;
using Chapter6.DataAccess.Repositories;
using Chapter6.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Chapter6.DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _context;

	public UnitOfWork(AppDbContext context)
	{
		_context = context;
		Cars = new CarRepository(_context);
		Schedules = new ScheduleRepository(_context);
		Drivers = new DriverRepository(_context);
		Destinations = new GenericRepository<Destination>(_context);
		SeatBookings = new GenericRepository<SeatBooking>(_context);
	}

	public ICarRepository Cars { get; }
	public IScheduleRepository Schedules { get; }
	public IDriverRepository Drivers { get; }
	public IGenericRepository<Destination> Destinations { get; }
	public IGenericRepository<SeatBooking> SeatBookings { get; }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

	public async Task<int> SaveChangeAsync() => await _context.SaveChangesAsync();

	public async Task ExecuteInTransactionAsync(Func<Task> action)
	{
		await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
		try
		{
			await action();
			await transaction.CommitAsync();
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
	}
}
