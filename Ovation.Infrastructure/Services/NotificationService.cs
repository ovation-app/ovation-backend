using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Ovation.Application.Common.Interfaces;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.ApiUser;
using Ovation.Application.Repositories;

namespace Ovation.Persistence.Services
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public sealed class NotificationService(IHttpContextAccessor httpContextAccessor, INotificationRepository notificationRepository) : Hub<INotificationService>
    {
        public async override Task OnConnectedAsync()
        {
            var user = getUser();
            if (user != null)
            {
                Constant._userIdToConnectionId[user.UserId] = Context.ConnectionId;

                await releaseQueuedNotification(user.UserId, Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var user = getUser();
            if (user != null && Constant._userIdToConnectionId.ContainsKey(user.UserId))
            {
                Constant._userIdToConnectionId.TryRemove(user.UserId, out _);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task MessageNotification(string userId, string message)
        {
            if (Constant._userIdToConnectionId.TryGetValue(new Guid(userId), out string connectionId) && Clients != null)
            {
                await Clients.Client(connectionId).MessageNotification(userId, message);
            }
            else
            {
                // Queue message for later delivery
                if (!Constant._offlineMessageNotification.ContainsKey(new Guid(userId)))
                {
                    Constant._offlineMessageNotification[new Guid(userId)] = new List<string>();
                }
                Constant._offlineMessageNotification[new Guid(userId)].Add(message);
            }
        }

        private async Task releaseQueuedNotification(Guid userId, string connectionId)
        {
            if (Constant._offlineNotification.TryGetValue(userId, out var messages))
            {
                foreach (var message in messages)
                {
                    await Clients.Client(connectionId).ReceivedNotification(message);
                }

                // Use TryRemove for thread-safe removal
                Constant._offlineNotification.TryRemove(userId, out _);
            }
        }

        internal async Task<ResponseData> GetUserNotificationsAsync(Guid userId, int page)
        {
            var response = await notificationRepository.GetUserNotificationsAsync(userId, page);

            return response;
        }

        internal async Task<ResponseData> GetUserPendingNotificationAsync(Guid userId)
        {
            var response = await notificationRepository.GetUserPendingNotificationAsync(userId);

            return response;
        }

        internal async Task<ResponseData> ReadNotificationAsync(Guid notificationId, Guid userId)
        {
            var response = await notificationRepository.ReadNotificationAsync(notificationId, userId);

            return response;
        }

        internal async Task<ResponseData> ReadAllNotificationAsync(Guid userId)
        {
            var response = await notificationRepository.ReadAllNotificationsAsync(userId);

            return response;
        }

        private UserPayload? getUser()
        {
            var user = new UserPayload();
            if (httpContextAccessor != null && httpContextAccessor.HttpContext != null)
            {
                try
                {
                    var payload = httpContextAccessor.HttpContext.User;
                    user.Email = payload.FindFirst("Email").Value;
                    user.UserId = Guid.Parse(payload.FindFirst("UserId").Value);
                    user.Username = payload.FindFirst("Username").Value;
                }
                catch (Exception)
                {
                    user = null;
                }
            }
            return user;
        }
    }
}
