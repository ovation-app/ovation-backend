using Ovation.Application.DTOs.SignUp;
using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class ProfileModDto
    {
        [Required]
        public string DisplayName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty!;

        [Required]
        public string Username { get; set; } = string.Empty!;

        public string? BirthDate { get; set; }

        public string? Location { get; set; }

        public string? Bio { get; set; }

        public string? ProfileImage { get; set; }

        public string? CoverImage { get; set; }

        public UserPathDto? UserPath { get; set; }
    }
}
