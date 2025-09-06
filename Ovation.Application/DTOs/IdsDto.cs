using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class IdsDto
    {
        [Required]
        public List<GuidId> Ids { get; set; } = null!;
    }
}
