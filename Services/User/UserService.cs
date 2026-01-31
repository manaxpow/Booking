public class UserService : IUserService
{
    private readonly IUserStore _userStore;

    public UserService(IUserStore userStore)
    {
        _userStore = userStore;
    }

    public void CreateUser(CreateUserRequest request)
    {
        // check if user already exists
        if (_userStore.Users.Any(u => u.Username == request.Username))
        {
            throw new Exception("User already exists.");
        }

        // create new user
        var newUser = new User
        {
            FullName = request.FullName,
            Username = request.Username,
            PasswordHash = request.Password // In real applications, hash the password
        };

        _userStore.Users.Add(newUser);
    }

    public void DeleteUser(string username)
    {
        var user = _userStore.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        _userStore.Users.Remove(user);
    }

    public User GetUserDetails(string username)
    {
        var user = _userStore.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        return user;
    }

    public void UpdateUser(UpdateUserRequest request, string username)
    {
        var user = _userStore.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        if (request.FullName != null)
        {
            user.FullName = request.FullName;
        }

        if (request.Password != null)
        {
            user.PasswordHash = request.Password; // In real applications, hash the password
        }
    }

    public bool ValidateUser(string username, string password)
    {
        var user = _userStore.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        return user.PasswordHash == password; // In real applications, hash and compare the password
    }

    public List<User> GetAllUsers()
    {
        return _userStore.Users;
    }
}