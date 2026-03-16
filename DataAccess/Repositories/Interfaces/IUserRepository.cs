public interface IUserRepository : IGenericRepository<User>
{
    public Task<User?> GetByPhoneAsync(string phoneNumber);
    public Task<User?> GetByEmailAsync(string email);
    public Task<(IEnumerable<User> Users, int TotalCount)> GetPagedUsersAsync(
        string? keyword,
        string? role,
        string? phone,
        string? email,
        int page,
        int pageSize);
}