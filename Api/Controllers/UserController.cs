using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("create")]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        _userService.CreateUser(request);
        return Ok("User created successfully.");
    }

    [HttpPut("update/{username}")]
    public IActionResult UpdateUser(string username, [FromBody] UpdateUserRequest request)
    {
        _userService.UpdateUser(request, username);
        return Ok("User updated successfully.");
    }

    [HttpPost("validate")]
    public IActionResult ValidateUser(string username, string password)
    {
        bool isValid = _userService.ValidateUser(username, password);
        return Ok(isValid);
    }

    [HttpDelete("delete/{username}")]
    public IActionResult DeleteUser(string username)
    {
        _userService.DeleteUser(username);
        return Ok("User deleted successfully.");
    }

    [HttpGet("details/{username}")]
    [Authorize]
    public IActionResult GetUserDetails(string username)
    {
        var userDetails = _userService.GetUserDetails(username);
        return Ok(userDetails);
    }

    [HttpGet("all")]
    public IActionResult GetAllUsers()
    {
        var users = _userService.GetAllUsers();
        return Ok(users);
    }
}