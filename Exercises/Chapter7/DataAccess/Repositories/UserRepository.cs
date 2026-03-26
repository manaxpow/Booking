using Chapter6.DataAccess.Context;
using Chapter6.Models;
using Microsoft.EntityFrameworkCore;

namespace Chapter6.DataAccess.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context)
        : base(context) { }

    public async Task<User?> GetByPhoneAsync(string phone) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
}
