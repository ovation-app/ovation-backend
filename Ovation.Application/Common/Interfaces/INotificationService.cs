using Ovation.Application.DTOs;

namespace Ovation.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task ReceivedNotification(NotificationDto notificationDto);

        Task MessageNotification(string userId, string message);
    }
}
