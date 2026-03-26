using Chapter6.DTOs.Users;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public interface IUserService
{
    Task<ServiceResult<GetUserByIdResponse>> GetByIdAsync(int id);
}
