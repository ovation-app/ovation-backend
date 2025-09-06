using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs.SignUp
{
    public class RegisterDto
    {
        [Required]
        public PersonalInfo PersonalInfo { get; set; } = null!;

        //public UserPathDto? UserPath { get; set; }

        public List<WalletAcct>? UserWallet { get; set; }

        public string Type { get; set; } = SignUp.Type.Normal.ToString();

        public string? Referral { get; set; }
    }

    public enum Type : byte
    {
        Normal,
        Google,
        X
    }
}
