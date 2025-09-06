using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Commands
{
    internal class UpdateNftSaleCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<UpdateNftSaleCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(UpdateNftSaleCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.UpdateNftSaleAsync(request.UserId, request.NftId, request.SaleDto);
        }
    }
} 