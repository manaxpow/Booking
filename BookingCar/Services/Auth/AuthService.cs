using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtOptions)
    {
        _unitOfWork = unitOfWork;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<string> LoginAsync(string phoneNumber, string password)
    {
        var user = await _unitOfWork.Users.GetByPhoneAsync(phoneNumber);
        if (user == null) return "Invalid phone number or password";

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (isPasswordValid)
            return GenerateJwtToken(user);
        return "Invalid phone number or password";
    }

    public async Task<string> RegisterAsync(string phoneNumber, string password, string fullName, string email, string cccd)
    {
        var existingUser = await _unitOfWork.Users.GetByPhoneAsync(phoneNumber);
        if (existingUser != null) return "Phone number already registered";

        var existingEmail = await _unitOfWork.Users.GetByEmailAsync(email);
        if (existingEmail != null) return "Email already registered";

        var user = new User
        {
            Phone = phoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FullName = fullName,
            Email = email,
            Cccd = cccd,
            Role = "USER",
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow
        };
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangeAsync();
        return "Registration successful";
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role ?? "User"),

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddDays(1), // Token hết hạn sau 1 ngày
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}