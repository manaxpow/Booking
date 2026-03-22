public interface ISeatService
{
    Task<bool> IsSeatNameUniqueAsync(string name, int carId);
}