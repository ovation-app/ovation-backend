using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.SignUp.Claimable
{
    public class ClaimableProfile
    {
        [JsonPropertyName("form_submission")]
        public FormSubmission FormSubmission { get; set; } = null!;

        [JsonPropertyName("profile_name")]
        public string ProfileName { get; set; } = string.Empty!;

        [JsonPropertyName("wallet_details")]
        public WalletDetails WalletDetails { get; set; } = null!;
    }

    public class WalletDetails
    {
        [JsonPropertyName("wallet_address")]
        public string WalletAddress { get; set; } = string.Empty!;

        [JsonPropertyName("chain_name")]
        public string ChainName { get; set; } = string.Empty!;
    }

    public class FormSubmission
    {
        [JsonPropertyName("submission_id")]
        public string SubmissionId { get; set; } = string.Empty!;

        [JsonPropertyName("submitted_at")]
        public DateTime SubmittedAt { get; set; }

        [JsonPropertyName("submission_type")]
        public string SubmissionType { get; set; } = string.Empty!;
    }
}
