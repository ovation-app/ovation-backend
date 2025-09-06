using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.NotificationFeatures.Requests.Commands;

namespace Ovation.Application.Features.NotificationFeatures.Handlers.Commands
{
    internal class ReadNotificationCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<ReadNotificationCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(ReadNotificationCommandRequest request, CancellationToken cancellationToken)
        {
            return await _notificationRepository.ReadNotificationAsync(request.Id, request.UserId);
        }
    }
}
