namespace Ovation.Application.DTOs
{
    public class NotificationDto
    {
        public string Reference { get; set; } = null!;

        public string? ReferenceId { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public Guid? InitiatorId { get; set; }

        public Guid ReceiverId { get; set; }
    }
}
