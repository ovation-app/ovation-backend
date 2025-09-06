using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.NotificationFeatures.Requests.Queries
{
    public sealed record GetNotificationsQueryRequest(Guid UserId, int Page) : IRequest<ResponseData>;
}
