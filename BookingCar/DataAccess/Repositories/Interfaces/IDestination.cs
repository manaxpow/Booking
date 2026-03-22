using Models.Dtos.Destination;

namespace DataAccess.Repositories.Interfaces;

public interface IDestinationRepository : IGenericRepository<Destination>
{
    public Task<(IEnumerable<Destination> Destinations, int TotalCount)>
    GetPagedDestinationsAsync(DestinationQuery query);
    public Task<Destination?> GetByIdAsync(int id);
    public Task AddAsync(Destination destination);
    public Task UpdateAsync(Destination destination);
    public Task DeleteAsync(Destination destination);
    public Task<DestinationResponse?> ExistsByProvince(string province);
    public Task<bool> HasRelatedSchedulesAsync(int destinationId);

}