using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.NotificationFeatures.Requests.Queries;

namespace Ovation.Application.Features.NotificationFeatures.Handlers.Queries
{
    internal class GetNotificationCountQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetNotificationCountQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetNotificationCountQueryRequest request, CancellationToken cancellationToken)
        {
            return await _notificationRepository.GetUserPendingNotificationAsync(request.UserId);
        }
    }
}
