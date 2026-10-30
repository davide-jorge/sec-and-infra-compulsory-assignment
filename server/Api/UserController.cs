using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Dtos;

namespace Api;


[ApiController]
[Route("[controller]")]
public class UserController(UserService service) : ControllerBase
{
    
    [HttpPost(nameof(CreateUser))]
    public UserDto CreateUser([FromBody] CreateUserRequestDto dto)
    {
        return service.CreateUser(dto);
        
    }

    [HttpDelete("{id}")]
    public void DeleteUser([FromRoute]string id)
    {
        service.DeleteUser(id);
    }
    
    [HttpPatch(nameof(UpdateUser))]
    public UserDto UpdateUser(
        [FromBody] UpdateUserRequestDto dto)
    {
        return service.UpdateUser(dto);
    }
}