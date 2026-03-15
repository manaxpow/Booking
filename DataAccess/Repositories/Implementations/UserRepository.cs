using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByPhoneAsync(string phoneNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Phone == phoneNumber);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedUsersAsync(
        string? keyword,
        string? role,
        string? phone,
        string? email,
        int page,
        int pageSize)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(u =>
                u.FullName.Contains(keyword) ||
                u.Phone.Contains(keyword) ||
                u.Email.Contains(keyword) ||
                u.Cccd.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(u => u.Role == role);
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            query = query.Where(u => u.Phone.Contains(phone));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(u => u.Email.Contains(email));
        }

        int totalCount = await query.CountAsync();

        var users = await query.OrderByDescending(u => u.Id)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

        return (users, totalCount);
    }
}