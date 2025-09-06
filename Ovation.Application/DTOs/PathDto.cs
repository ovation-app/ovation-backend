using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class PathDto
    {
        [Required]
        public string Name { get; set; } = string.Empty!;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
