using Chapter6.DataAccess.UnitOfWork;
using Chapter6.DTOs.Destinations;
using Chapter6.Models;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public class DestinationService(IUnitOfWork unitOfWork) : IDestinationService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ServiceResult<CreateDestinationResponse>> CreateAsync(CreateDestinationRequest request)
    {
        var normalizedName = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return ServiceResult<CreateDestinationResponse>.BadRequest("Tên destination không được để trống.");
        }

        var existed = await _unitOfWork.Destinations.FindAsync(d => d.Name.ToLower() == normalizedName.ToLower());
        if (existed.Any())
        {
            return ServiceResult<CreateDestinationResponse>.Conflict($"Destination '{normalizedName}' đã tồn tại.");
        }

        var destination = new Destination
        {
            Name = normalizedName,
            Address = request.Address?.Trim(),
            IsActive = true
        };

        await _unitOfWork.Destinations.AddAsync(destination);
        await _unitOfWork.SaveChangeAsync();

        return ServiceResult<CreateDestinationResponse>.Success(new CreateDestinationResponse
        {
            Id = destination.Id
        });
    }

    public async Task<ServiceResult<GetDestinationByIdResponse>> GetByIdAsync(int id)
    {
        var destination = await _unitOfWork.Destinations.GetByIdAsync(id);
        if (destination is null)
        {
            return ServiceResult<GetDestinationByIdResponse>.NotFound("Destination not found.");
        }

        return ServiceResult<GetDestinationByIdResponse>.Success(new GetDestinationByIdResponse
        {
            Id = destination.Id,
            Name = destination.Name,
            Address = destination.Address,
            IsActive = destination.IsActive
        });
    }
}