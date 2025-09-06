using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class UserExperienceDto
    {
        [Required]
        public string Company { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;

        [Required]
        public string Department { get; set; } = null!;

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? Description { get; set; }

        public string? Skill { get; set; }
    }
}
