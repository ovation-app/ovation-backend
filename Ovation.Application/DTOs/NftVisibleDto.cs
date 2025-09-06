using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class NftVisibleDto
    {
        [Required]
        public int NftId { get; set; }

        [Required]
        public bool Public { get; set; }
    }
}
