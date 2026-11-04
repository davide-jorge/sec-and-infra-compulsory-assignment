using Infrastructure;
using LinqToDB;
using Microsoft.VisualBasic.CompilerServices;
using Service.Dtos;
using Service.Security;
using Service.Authorization;

namespace Service;
//todo use ENUMS FOR User;SuperUser,Admin
public class UserService (myDbConnection db,IPasswordHasher passwordHasher,
    IAuthorizationService authorizationService)
{
    public UserDto CreateUser(CreateUserRequestDto dto,string currentUserId)
    {
        
        var currentUser = db.Users
            .FirstOrDefault(u => u.UserId == currentUserId);

        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found.");
        
        // Only admins and superusers can create users
        if (currentUser.Role != "admin" && currentUser.Role != "superuser")
            throw new UnauthorizedAccessException("No permission to create users.");
        
        if (currentUser.Role == "superuser" && dto.Role != "user")
            throw new UnauthorizedAccessException("Superusers can only create regular users.");
        
        if (dto.Role != "user" && dto.Role != "superuser" && dto.Role != "admin")
            throw new InvalidOperationException("Invalid role.");
        
        
        var existingUser = db.Users
            .FirstOrDefault(u => u.Username == dto.Username);

        if (existingUser != null)
            throw new InvalidOperationException("Username already exists.");
        
        
        string passwordHash = passwordHasher.HashAndSaltPassword(dto.Password);

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = dto.Username,
            PasswordHash = passwordHash,
            Role = dto.Role
        };

        db.Insert(user);
        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role
        };
        
    }

    public void DeleteUser(string id, string currentUserId)
    {
        var currentUser = db.Users
            .FirstOrDefault(u => u.UserId == currentUserId);
        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found.");
        var userToDelete = db.Users
            .FirstOrDefault(u => u.UserId == id);

        if (userToDelete == null)
            throw new KeyNotFoundException("User not found.");
        
        // Checks if user is admin or superuser to delete users
        if (currentUser.Role != "admin" && currentUser.Role != "superuser")
            throw new UnauthorizedAccessException("This user has no permission to delete users.");
        
        
        //Checks if its Superuser to delete normal users
        
        if (currentUser.Role == "superuser" && userToDelete.Role != "user")
            throw new UnauthorizedAccessException(
                "Superusers can only delete regular users.");
        

        db.Users.Delete(u => u.UserId == id);
    }

    public UserDto UpdateUser(UpdateUserRequestDto dto, string currentUserId)
    {
        var currentUser = db.Users
            .FirstOrDefault(u => u.UserId == currentUserId);

        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found.");
        var userToUpdate = db.Users
            .FirstOrDefault(u => u.UserId == dto.UserId);
        if (userToUpdate == null)
            throw new KeyNotFoundException("User not found.");
        
        if (currentUser.Role != "admin" && currentUser.Role != "superuser")
            throw new UnauthorizedAccessException("This user has no permission to update users.");
      
        if (currentUser.Role == "superuser" && userToUpdate.Role != "user")
            throw new UnauthorizedAccessException("Superusers can only update regular users.");
   
        if (currentUser.Role == "superuser" && dto.Role != "user")
            throw new UnauthorizedAccessException("Superusers can only manage regular users.");
        
        if (dto.Role != "user" && dto.Role != "superuser" && dto.Role != "admin")
            throw new InvalidOperationException("Invalid role.");
        
        db.Users
            .Where(u => u.UserId == dto.UserId)
            .Set(u => u.Username, dto.Username)
            .Set(u => u.PasswordHash, passwordHasher.HashAndSaltPassword(dto.Password))
            .Set(u => u.Role, dto.Role)
            .Update();

        return new UserDto
        {
            UserId = dto.UserId,
            Username = dto.Username,
            Role = dto.Role
        };
        
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

        bool passwordIsValid = passwordHasher.VerifyPassword(
            dto.Password,
            user.PasswordHash
            
            );

        if (!passwordIsValid)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        return new LoginResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role,
            Token = authorizationService.GenerateToken(user)
        };
    }
}
