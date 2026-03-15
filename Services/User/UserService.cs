using VehicleBooking.Models.DTOs.Common;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<UserResponse>> GetUsersAsync(UserQueryParameters query)
    {
        var (users, totalCount) = await _unitOfWork.Users.GetPagedUsersAsync(
            query.Keyword,
            query.Role,
            query.Phone,
            query.Email,
            query.Page,
            query.PageSize);

        var items = users.Select(u => new UserResponse(
            u.Id,
            u.FullName,
            u.Phone,
            u.Email,
            u.Cccd,
            u.Role,
            u.CreateAt,
            u.UpdateAt)).ToList();

        return new PagedResult<UserResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<UserDetailResponse?> GetUserByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return null;

        return new UserDetailResponse(
            user.Id,
            user.FullName,
            user.Phone,
            user.Email,
            user.Cccd,
            user.Role,
            user.CreateAt,
            user.UpdateAt);
    }

    public async Task CreateUserAsync(CreateUserRequest request)
    {
        var existingPhone = await _unitOfWork.Users.GetByPhoneAsync(request.Phone);
        if (existingPhone != null)
        {
            throw new Exception("Phone number already exists.");
        }

        var existingEmail = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existingEmail != null)
        {
            throw new Exception("Email already exists.");
        }

        var user = new User
        {
            FullName = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Cccd = request.Cccd,
            Role = request.Role,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateUserAsync(UpdateUserRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.Phone) && request.Phone != user.Phone)
        {
            var existingPhone = await _unitOfWork.Users.GetByPhoneAsync(request.Phone);
            if (existingPhone != null && existingPhone.Id != user.Id)
            {
                throw new Exception("Phone number already exists.");
            }

            user.Phone = request.Phone;
        }

        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            var existingEmail = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (existingEmail != null && existingEmail.Id != user.Id)
            {
                throw new Exception("Email already exists.");
            }

            user.Email = request.Email;
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.FullName = request.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        if (!string.IsNullOrWhiteSpace(request.Cccd))
        {
            user.Cccd = request.Cccd;
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            user.Role = request.Role;
        }

        user.UpdateAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        _unitOfWork.Users.Remove(user);
        await _unitOfWork.CompleteAsync();
    }
}
