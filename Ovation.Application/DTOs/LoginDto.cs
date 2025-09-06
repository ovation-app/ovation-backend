using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty!;
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty!;
    }
}
