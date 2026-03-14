public interface ICarRepository : IGenericRepository<Car>
{
    public Task<Car?> GetByLicensePlateAsync(string licensePlate);
    public Task<Car?> GetCarWithSeatsByIdAsync(int id);
    public Task<(IEnumerable<Car> Cars, int TotalCount)> GetPagedCarsAsync(string? keyword, string? brand, int page, int pageSize);
}