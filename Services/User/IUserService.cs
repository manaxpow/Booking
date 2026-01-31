public interface IUserService
{
    void CreateUser(CreateUserRequest request);
    void UpdateUser(UpdateUserRequest request, string username);
    bool ValidateUser(string username, string password);
    void DeleteUser(string username);
    User GetUserDetails(string username);
    List<User> GetAllUsers();
}