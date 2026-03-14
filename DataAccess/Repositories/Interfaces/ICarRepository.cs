public interface ICarRepository : IGenericRepository<Car>
{
    public Task<Car?> GetByLicensePlateAsync(string licensePlate);
    public Task<Car?> GetCarWithSeatsByIdAsync(int id);
}