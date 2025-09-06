using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class SocialLoginDto
    {
        [Required]
        public string SocialId { get; set; } = string.Empty!;
    }
}
