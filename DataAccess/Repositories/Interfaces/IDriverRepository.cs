public interface IDriverRepository : IGenericRepository<Driver>
{
    Task<(IEnumerable<Driver> Drivers, int TotalCount)> GetPagedDriversAsync(
        string? keyword,
        string? license,
        DateTime? dobFrom,
        DateTime? dobTo,
        int page,
        int pageSize);
}
