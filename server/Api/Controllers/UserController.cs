using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Dtos;

namespace Api;


[ApiController]
[Route("[controller]")]
public class UserController(UserService service) : ControllerBase
{
    [Authorize(Roles = "superuser,admin")]
    [HttpPost(nameof(CreateUser))]
    public UserDto CreateUser([FromBody] CreateUserRequestDto dto)
    
    
    
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
            throw new UnauthorizedAccessException("Missing user ID.");

        return service.CreateUser(dto,currentUserId);
        
    }

    [Authorize(Roles = "superuser,admin")]
    [HttpDelete("{id}")]
    public void DeleteUser([FromRoute]string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
            throw new UnauthorizedAccessException("Missing user ID.");
        
        service.DeleteUser(id,currentUserId);
    }
    
    [Authorize(Roles = "superuser,admin")]
    [HttpPatch(nameof(UpdateUser))]
    public UserDto UpdateUser(
        [FromBody] UpdateUserRequestDto dto)
    {
        
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
            throw new UnauthorizedAccessException("Missing user ID.");

        return service.UpdateUser(dto, currentUserId);
    }
    
    
    [Authorize]
    [HttpGet(nameof(GetUsers))]
    public List<UserDto> GetUsers()
    {
        return service.GetUsers();
    }
    
    
    
    [HttpPost(nameof(Login))]
    public LoginResponseDto Login([FromBody] LoginRequestDto dto)
    {
        return service.Login(dto);
    }
    
}