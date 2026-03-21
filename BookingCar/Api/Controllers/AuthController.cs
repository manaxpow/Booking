using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.PhoneNumber, request.Password);
        if (result.StartsWith("Invalid"))
            return BadRequest(result);
        return Ok(new { Token = result });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request.PhoneNumber, request.Password, request.FullName, request.Email, request.Cccd);
        if (result != "Registration successful")
            return BadRequest(result);
        return Ok(result);
    }
}