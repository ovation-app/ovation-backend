using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Features.NewsletterFeatures.Requests.Commands;

namespace Ovation.Application.Features.NewsletterFeatures.Handlers.Commands
{
    internal class NuclearPlayGroundCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<NuclearPlayGroundCommandRequst>
    {
        public async Task<Unit> Handle(NuclearPlayGroundCommandRequst request, CancellationToken cancellationToken)
        {
            await _nuclearPlayGroundRepository.NFTDataAsync(request.Page);

            return new();
        }
    }
}
