using System.ComponentModel.DataAnnotations;

namespace Service.Dtos;

public class LoginRequestDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
    
}