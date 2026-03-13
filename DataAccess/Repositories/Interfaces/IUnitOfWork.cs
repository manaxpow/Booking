public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;
    IUserRepository Users { get; }
    Task<int> CompleteAsync();
    Task SaveChangeAsync();
}