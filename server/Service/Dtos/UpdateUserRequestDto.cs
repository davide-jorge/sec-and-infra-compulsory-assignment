using System.ComponentModel.DataAnnotations;

namespace Service.Dtos;

public class UpdateUserRequestDto
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string Role { get; set; }
}