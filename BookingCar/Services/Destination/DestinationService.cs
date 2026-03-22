using Models.Dtos.Destination;
using VehicleBooking.Models.DTOs.Common;
using DataAccess.Repositories.Interfaces;

public class DestinationService : IDestinationService
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DestinationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _destinationRepository = unitOfWork.Destinations;
    }

    public async Task<PagedResult<DestinationResponse>> GetDestinationsAsync(DestinationQuery query)
    {
        var (destinations, totalCount) = await _destinationRepository.GetPagedDestinationsAsync(query);
        var destinationResponses = destinations.Select(d => new DestinationResponse
            (d.Id,
             d.Province,
             d.CreateAt,
             d.UpdateAt))
            .ToList();
        return new PagedResult<DestinationResponse>
        {
            Items = destinationResponses,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task CreateDestinationAsync(CreateDestinationRequest request)
    {
        // Check for duplicate province
        var existingDestination = await _destinationRepository.ExistsByProvince(request.Province);
        if (existingDestination != null)
            throw new InvalidOperationException("Địa điểm này đã tồn tại.");

        var destination = new Destination
        {
            Province = request.Province,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow
        };

        await _destinationRepository.AddAsync(destination);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task UpdateDestinationAsync(UpdateDestinationRequest request, int id)
    {
        var existingDestination = await _destinationRepository.GetByIdAsync(id);
        if (existingDestination == null)
            throw new KeyNotFoundException("Không tìm thấy địa điểm.");

        // Only update if province is provided and different
        if (!string.IsNullOrEmpty(request.Province) && request.Province != existingDestination.Province)
        {
            // Check for duplicate province
            var duplicateDestination = await _destinationRepository.ExistsByProvince(request.Province);
            if (duplicateDestination != null && duplicateDestination.Id != id)
                throw new InvalidOperationException("Địa điểm này đã tồn tại.");

            existingDestination.Province = request.Province;
            existingDestination.UpdateAt = DateTime.UtcNow;
        }

        await _destinationRepository.UpdateAsync(existingDestination);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task DeleteDestinationAsync(int id)
    {
        var existingDestination = await _destinationRepository.GetByIdAsync(id);
        if (existingDestination == null)
            throw new KeyNotFoundException("Không tìm thấy địa điểm.");

        var hasRelatedSchedules = await _destinationRepository.HasRelatedSchedulesAsync(id);
        if (hasRelatedSchedules)
            throw new InvalidOperationException("Địa điểm này đang được sử dụng trong lịch trình, không thể xóa.");

        await _destinationRepository.DeleteAsync(existingDestination);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task<DestinationResponse?> GetDestinationByIdAsync(int id)
    {
        var destination = await _destinationRepository.GetByIdAsync(id);
        if (destination == null) return null;
        var destinationResponse =
            new DestinationResponse(destination.Id, destination.Province, destination.CreateAt, destination.UpdateAt);
        return destinationResponse;
    }
}