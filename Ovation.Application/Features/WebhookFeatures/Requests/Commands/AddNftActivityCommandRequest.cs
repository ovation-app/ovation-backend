using MediatR;

namespace Ovation.Application.Features.WebhookFeatures.Requests.Commands
{
    public sealed record AddNftActivityCommandRequest(object Data) : IRequest;
}
