using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.FeedbackFeatures.Requests.Commands
{
    public sealed record AddFeedbackCommandRequest(UserFeedbackDto UserFeedback, Guid UserId) : IRequest<ResponseData>;
}
