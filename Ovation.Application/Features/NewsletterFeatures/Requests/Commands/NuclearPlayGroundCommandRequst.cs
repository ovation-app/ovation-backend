using MediatR;

namespace Ovation.Application.Features.NewsletterFeatures.Requests.Commands
{
    public sealed record NuclearPlayGroundCommandRequst(int Page) : IRequest;
}
