using Ovation.Application.DTOs.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class IdsWithType
    {
        [Required]
        public Guid Id { get; set; } = Guid.Empty!;
        [Required]
        public string Type { get; set; } = FeaturedType.NFT.ToString();
    }

    public class HexWithType
    {
        [Required]
        public string Id { get; set; } = string.Empty!;
        [Required]
        public string Type { get; set; } = string.Empty!;
    }
}
