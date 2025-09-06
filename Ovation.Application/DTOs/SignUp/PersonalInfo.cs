using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs.SignUp
{
    public class PersonalInfo
    {
        [Required]
        public string DisplayName { get; set; } = string.Empty!;

        //[EmailAddress(ErrorMessage = "Invalid email address"), Required]
        public string? Email { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty!;

        public string? Password { get; set; }

        public string? ImgUrl { get; set; }

        public string? XId { get; set; }
    }
}
