using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public Guid UserId { get; set; } = Guid.Empty!;

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty!;
    }
}
