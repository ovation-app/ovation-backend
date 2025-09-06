using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs.SignUp
{
    public class WalletAcct
    {
        public Guid? WalletTypeId { get; set; }

        public List<AddressData> Data { get; set; } = new()!;
    }

    public class AddressData
    {
        [Required]
        public string WalletAddress { get; set; } = string.Empty!;

        [Required, MinLength(2)]
        public string Chain { get; set; } = string.Empty!;
    }

    public sealed record WalletNftsCount(string Chain, int Count);
}
