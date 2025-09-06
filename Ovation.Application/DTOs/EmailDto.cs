using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class EmailDto
    {
        [Required]
        public string Email { get; set; } = string.Empty!;
    }
}
