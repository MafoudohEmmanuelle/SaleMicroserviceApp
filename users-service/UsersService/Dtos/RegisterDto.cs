using System.ComponentModel.DataAnnotations;
using UserService.Models;

namespace UserService.Dtos
{
    public class RegisterDto
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Required] public UserRole Role { get; set; }  // Enum
    }
}
