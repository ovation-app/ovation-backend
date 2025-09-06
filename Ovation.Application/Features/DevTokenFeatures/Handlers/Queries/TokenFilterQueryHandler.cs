using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Features.DevTokenFeatures.Requests.Queries;

namespace Ovation.Application.Features.DevTokenFeatures.Handlers.Queries
{
    internal class TokenFilterQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<TokenFilterQueryRequest, bool>
    {
        public async Task<bool> Handle(TokenFilterQueryRequest request, CancellationToken cancellationToken)
        {
            return await _devRepository.VerifyToken(request.TokenId);
        }
    }
}
