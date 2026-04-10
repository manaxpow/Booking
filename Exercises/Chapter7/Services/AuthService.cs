using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Chapter6.Configuration;
using Chapter6.DataAccess.UnitOfWork;
using Chapter6.DTOs.Auth;
using Chapter6.Models;
using Chapter6.Services.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Chapter6.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;
    private readonly IRefreshTokenStore _refreshTokenStore;

    public AuthService(
        IUnitOfWork unitOfWork,
        IOptions<JwtSettings> jwtOptions,
        IRefreshTokenStore refreshTokenStore)
    {
        _unitOfWork = unitOfWork;
        _jwtSettings = jwtOptions.Value;
        _refreshTokenStore = refreshTokenStore;
    }

    public async Task<ServiceResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        var normalizedPhone = request.Phone.Trim();
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedRole = string.IsNullOrWhiteSpace(request.Role)
            ? "USER"
            : request.Role.Trim().ToUpperInvariant();

        if (normalizedRole is not ("USER" or "ADMIN"))
        {
            return ServiceResult<RegisterResponse>.BadRequest(
                "Role chỉ chấp nhận USER hoặc ADMIN."
            );
        }

        var existedByPhone = await _unitOfWork.Users.GetByPhoneAsync(normalizedPhone);
        if (existedByPhone is not null)
        {
            return ServiceResult<RegisterResponse>.Conflict(
                $"Số điện thoại '{normalizedPhone}' đã tồn tại."
            );
        }

        var existedByEmail = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);
        if (existedByEmail is not null)
        {
            return ServiceResult<RegisterResponse>.Conflict(
                $"Email '{normalizedEmail}' đã tồn tại."
            );
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Phone = normalizedPhone,
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Cccd = request.Cccd.Trim(),
            Role = normalizedRole,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow,
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        return ServiceResult<RegisterResponse>.Success(
            new RegisterResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                Cccd = user.Cccd,
                Role = user.Role,
                CreateAt = user.CreateAt,
            }
        );
    }

    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var normalizedPhone = request.Phone.Trim();
        var user = await _unitOfWork.Users.GetByPhoneAsync(normalizedPhone);

        if (user is null)
        {
            return ServiceResult<LoginResponse>.BadRequest("Sai số điện thoại hoặc mật khẩu.");
        }

        var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValidPassword)
        {
            return ServiceResult<LoginResponse>.BadRequest("Sai số điện thoại hoặc mật khẩu.");
        }

        var (accessToken, expiresAtUtc) = GenerateJwtToken(user);
        var (refreshToken, refreshTokenExpiresAtUtc) = _refreshTokenStore.Generate(user.Id);

        return ServiceResult<LoginResponse>.Success(
            new LoginResponse
            {
                AccessToken = accessToken,
                ExpiresAtUtc = expiresAtUtc,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
            }
        );
    }

    public async Task<ServiceResult<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        if (!_refreshTokenStore.TryConsume(request.RefreshToken, out var userId))
        {
            return ServiceResult<RefreshTokenResponse>.BadRequest(
                "Refresh token không hợp lệ hoặc đã hết hạn."
            );
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user is null)
        {
            return ServiceResult<RefreshTokenResponse>.NotFound(
                "Người dùng không tồn tại."
            );
        }

        var (accessToken, expiresAtUtc) = GenerateJwtToken(user);
        var (newRefreshToken, refreshTokenExpiresAtUtc) = _refreshTokenStore.Generate(user.Id);

        return ServiceResult<RefreshTokenResponse>.Success(
            new RefreshTokenResponse
            {
                AccessToken = accessToken,
                ExpiresAtUtc = expiresAtUtc,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
            }
        );
    }

    private (string token, DateTime expiresAtUtc) GenerateJwtToken(User user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddDays(1);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        return (token, expiresAtUtc);
    }
}
