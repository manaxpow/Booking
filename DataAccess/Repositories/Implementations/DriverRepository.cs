using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

public class DriverRepository : GenericRepository<Driver>, IDriverRepository
{
    public DriverRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Driver> Drivers, int TotalCount)> GetPagedDriversAsync(
        string? keyword,
        string? license,
        DateTime? dobFrom,
        DateTime? dobTo,
        int page,
        int pageSize)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(d => d.Name.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(license))
        {
            query = query.Where(d => d.License == license);
        }

        if (dobFrom.HasValue)
        {
            var from = dobFrom.Value.Date;
            query = query.Where(d => d.Dob.Date >= from);
        }

        if (dobTo.HasValue)
        {
            var to = dobTo.Value.Date;
            query = query.Where(d => d.Dob.Date <= to);
        }

        int totalCount = await query.CountAsync();

        var drivers = await query.OrderByDescending(d => d.Id)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();

        return (drivers, totalCount);
    }
}
