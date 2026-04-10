using Chapter6.DTOs.Auth;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public interface IAuthService
{
    Task<ServiceResult<RegisterResponse>> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ServiceResult<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
}
