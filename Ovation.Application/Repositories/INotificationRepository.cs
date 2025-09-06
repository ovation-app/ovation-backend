using Ovation.Application.DTOs;
using Ovation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Repositories
{
    public interface INotificationRepository : IBaseRepository<UserNotification>
    {
        Task<ResponseData> GetUserNotificationsAsync(Guid userId, int page);
        Task<ResponseData> GetUserPendingNotificationAsync(Guid userId);
        Task<ResponseData> ReadNotificationAsync(Guid notificationId, Guid userId);
        Task<ResponseData> ReadAllNotificationsAsync(Guid userId);
    }
}
