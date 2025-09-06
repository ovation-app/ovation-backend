namespace Ovation.Application.DTOs
{
    public class UserFeedbackDto
    {
        public string? Satisfactory { get; set; }

        public string? UsefulFeature { get; set; }

        public string? Improvement { get; set; }

        public string? Confusion { get; set; }

        public string? LikelyRecommend { get; set; }

        public string? Addition { get; set; }

        public string? BiggestPain { get; set; }

        public string UserEmail { get; set; } = null!;
    }
}
