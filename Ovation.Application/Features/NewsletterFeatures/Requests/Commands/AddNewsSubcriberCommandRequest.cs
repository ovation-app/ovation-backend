using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.NewsletterFeatures.Requests.Commands
{
    public sealed record AddNewsSubcriberCommandRequest(NewsletterDto NewsletterDto) : IRequest<ResponseData>;
}
