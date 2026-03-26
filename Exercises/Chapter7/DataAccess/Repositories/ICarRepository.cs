using Chapter6.Models;

namespace Chapter6.DataAccess.Repositories;

public interface ICarRepository : IGenericRepository<Car>
{
    Task<Car?> GetByLicensePlateAsync(string licensePlate);
    Task<Car?> GetCarWithSeatsByIdAsync(int id);
    Task<IEnumerable<Car>> GetPagedCarsAsync(
        string? keyword,
        string? brand,
        int page,
        int pageSize
    );
}
