namespace FirstProjectDotNetCore.Endpoints.Users;

public class UserResponse
{
    public string Name { get; set; }
    public string Email { get; set; }

    public UserResponse(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
