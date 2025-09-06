using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class VerifiyUserDTO
    {
        [Required]
        public string VerificationType { get; set; } = string.Empty!;

        [Required]
        public string Handle { get; set; } = string.Empty!;

        public string TypeId { get; set; } = string.Empty!;
    }

    public enum VerificationTypes
    {
        X,
        Lens,
        Farcaster,
        ENS
    }
}
