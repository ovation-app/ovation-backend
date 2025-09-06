using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class PasswordModDto
    {
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty!;

        [Required, MinLength(8)]
        public string OldPassword { get; set; } = string.Empty!;
    }
}
