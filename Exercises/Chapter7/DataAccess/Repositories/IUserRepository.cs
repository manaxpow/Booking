using Chapter6.Models;

namespace Chapter6.DataAccess.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByPhoneAsync(string phone);
    Task<User?> GetByEmailAsync(string email);
}
