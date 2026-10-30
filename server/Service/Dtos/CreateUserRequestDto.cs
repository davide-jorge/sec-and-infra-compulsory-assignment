using System.ComponentModel.DataAnnotations;

namespace Service.Dtos;

public class CreateUserRequestDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string Role { get; set; }
}