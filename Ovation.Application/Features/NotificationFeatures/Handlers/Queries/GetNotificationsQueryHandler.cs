using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.NotificationFeatures.Requests.Queries;

namespace Ovation.Application.Features.NotificationFeatures.Handlers.Queries
{
    internal class GetNotificationsQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetNotificationsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetNotificationsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _notificationRepository.GetUserNotificationsAsync(request.UserId, request.Page);
        }
    }
}
