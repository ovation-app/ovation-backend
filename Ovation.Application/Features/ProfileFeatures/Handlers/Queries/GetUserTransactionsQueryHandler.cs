using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    class GetUserTransactionsQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserTransactionsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserTransactionsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserTransactionsDataAsync(request.UserId, request.Cursor);
        }
    }
}
