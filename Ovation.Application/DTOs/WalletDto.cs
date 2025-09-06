using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class WalletDto
    {
        [Required]
        public string Name { get; set; } = string.Empty!;

        public string? LogoUrl { get; set; }
    }
}
