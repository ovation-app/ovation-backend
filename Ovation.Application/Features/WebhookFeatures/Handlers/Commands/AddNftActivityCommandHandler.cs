using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Features.WebhookFeatures.Requests.Commands;

namespace Ovation.Application.Features.WebhookFeatures.Handlers.Commands
{
    internal class AddNftActivityCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<AddNftActivityCommandRequest>
    {
        public async Task<Unit> Handle(AddNftActivityCommandRequest request, CancellationToken cancellationToken)
        {
            await _webhookRepository.AddNftActivity(request.Data);
            return Unit.Value;
        }
    }
}
