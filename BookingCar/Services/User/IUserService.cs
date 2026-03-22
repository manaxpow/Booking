using VehicleBooking.Models.DTOs.Common;

public interface IUserService
{
    Task<PagedResult<UserResponse>> GetUsersAsync(UserQueryParameters query);
    Task<UserDetailResponse?> GetUserByIdAsync(int id);
    Task CreateUserAsync(CreateUserRequest request);
    Task UpdateUserAsync(UpdateUserRequest request);
    Task DeleteUserAsync(int id);
}
