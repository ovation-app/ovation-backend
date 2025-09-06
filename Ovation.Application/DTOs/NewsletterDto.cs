using System.ComponentModel.DataAnnotations;

namespace Ovation.Application.DTOs
{
    public class NewsletterDto
    {
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string SubscriberEmail { get; set; } = string.Empty!;
    }
}
