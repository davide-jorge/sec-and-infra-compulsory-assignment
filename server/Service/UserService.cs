using Infrastructure;
using Service.Dtos;

namespace Service;
//todo use ENUMS FOR User;SuperUser,Admin
public class UserService (myDbConnection db)
{
    public UserDto CreateUser(CreateUserRequestDto dto)
    {
        
        var existingUser = db.Users
            .FirstOrDefault(u => u.Username == dto.Username);

        if (existingUser != null)
            throw new InvalidOperationException("Username already exists.");


        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = dto.Username.Trim(),
            PasswordHash = "TEMPORARY",
            Role = "user"
        };
        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role
        };
        
    }

    public void DeleteUser(string id)
    {
        throw new NotImplementedException();
    }

    public UserDto UpdateUser(UpdateUserRequestDto dto)
    {
        throw new NotImplementedException();
    }
    
    public List<UserDto> GetUsers()
    {
        return db.Users
            .Select(u=> new UserDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Role = u.Role,
            })
            .ToList();
    }

    public LoginResponseDto Login(LoginRequestDto dto)
    {
        var user = db.Users
            .FirstOrDefault(u =>
                u.Username == dto.Username);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        
        //TODO implement the actual authentication
        if (user.Username == "admin1" && dto.Password == "admin")
        {
            return new LoginResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role,
            };
        }
        throw new UnauthorizedAccessException("Invalid username or password.");
    }
}
