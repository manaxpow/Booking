public interface IUserRepository : IGenericRepository<User>
{
    public Task<User?> GetByPhoneAsync(string phoneNumber);
    public Task<User?> GetByEmailAsync(string email);
}