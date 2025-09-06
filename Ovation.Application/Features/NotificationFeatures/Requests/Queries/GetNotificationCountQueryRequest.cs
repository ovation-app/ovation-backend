using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.NotificationFeatures.Requests.Queries
{
    public sealed record GetNotificationCountQueryRequest(Guid UserId) : IRequest<ResponseData>;
}
