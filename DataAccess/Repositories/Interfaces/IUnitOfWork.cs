public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;
    IUserRepository Users { get; }
    ICarRepository Cars { get; }
    ISeatRepository Seats { get; }
    Task<int> CompleteAsync();
    Task SaveChangeAsync();
}