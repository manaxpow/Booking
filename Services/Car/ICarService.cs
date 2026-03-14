public interface ICarService
{
    Task CreateCarAsync(CreateCarRequest request);
    Task UpdateCarAsync(UpdateCarRequest request);
    Task DeleteCarAsync(int id);
}