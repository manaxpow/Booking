public interface ISeatRepository : IGenericRepository<Seat>
{
    public Task<Seat?> GetByNameAsync(string name);
    public Task<Seat?> GetByCarIdAndNameAsync(int carId, string name);
}