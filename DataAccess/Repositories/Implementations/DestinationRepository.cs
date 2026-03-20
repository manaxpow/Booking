using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories.Implementations;

public class DestinationRepository : GenericRepository<Destination>, DataAccess.Repositories.Interfaces.IDestinationRepository
{
    public DestinationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Destination> Destinations, int TotalCount)> GetPagedDestinationsAsync(DestinationQuery query)
    {
        var destinationQuery = _dbSet.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Province))
        {
            destinationQuery = destinationQuery.Where(d => d.Province == query.Province);
        }
        var totalCount = await destinationQuery.CountAsync();
        var destinations = await destinationQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (destinations, totalCount);
    }

    public new async Task AddAsync(Destination destination)
    {
        await _dbSet.AddAsync(destination);
    }

    public async Task UpdateAsync(Destination destination)
    {
        _dbSet.Update(destination);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Destination destination)
    {
        _dbSet.Remove(destination);
        await Task.CompletedTask;
    }

    public async Task<DestinationResponse?> ExistsByProvince(string province)
    {
        var destination = await _dbSet.AsNoTracking().FirstOrDefaultAsync(d => d.Province == province);
        if (destination == null) return null;
        return new DestinationResponse(destination.Id, destination.Province, destination.CreateAt, destination.UpdateAt);
    }

    public Task<bool> HasRelatedSchedulesAsync(int destinationId)
    {
        return _context.Schedules.AnyAsync(s => s.FromDestinationId == destinationId || s.ToDestinationId == destinationId);
    }
}