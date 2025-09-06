using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.NotificationFeatures.Requests.Commands;

namespace Ovation.Application.Features.NotificationFeatures.Handlers.Commands
{
    internal class ReadAllNotificationsCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<ReadAllNotificationsCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(ReadAllNotificationsCommandRequest request, CancellationToken cancellationToken)
        {
            return await _notificationRepository.ReadAllNotificationsAsync(request.UserId);
        }
    }
}
