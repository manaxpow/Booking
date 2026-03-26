using Chapter6.DataAccess.UnitOfWork;
using Chapter6.DTOs.Users;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<GetUserByIdResponse>> GetByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
        {
            return ServiceResult<GetUserByIdResponse>.NotFound($"Không tìm thấy user id={id}.");
        }

        return ServiceResult<GetUserByIdResponse>.Success(
            new GetUserByIdResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                Cccd = user.Cccd,
                Role = user.Role,
                CreateAt = user.CreateAt,
                UpdateAt = user.UpdateAt,
            }
        );
    }
}
