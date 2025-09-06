using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Features.DevTokenFeatures.Requests.Queries;

namespace Ovation.Application.Features.DevTokenFeatures.Handlers.Queries
{
    internal class CoreTokenFilterQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<CoreTokenFilterQueryRequest, bool>
    {
        public async Task<bool> Handle(CoreTokenFilterQueryRequest request, CancellationToken cancellationToken)
        {
            return await _devRepository.VerifyCoreToken(request.TokenId);
        }
    }
}
