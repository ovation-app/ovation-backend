using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.NotificationFeatures.Requests.Commands
{
    public sealed record ReadAllNotificationsCommandRequest(Guid UserId) : IRequest<ResponseData>;
}
