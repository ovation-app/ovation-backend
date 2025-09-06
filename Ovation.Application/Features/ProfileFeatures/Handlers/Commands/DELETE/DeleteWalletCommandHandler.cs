using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.DELETE;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.DELETE
{
    internal class DeleteWalletCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<DeleteWalletCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(DeleteWalletCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _profileRepository.DeleteWalletAsync(request.WalletId, request.UserId);

            if (response.Status)
                await _domainEvent.WalletDeletedEvent(request.WalletId, string.Empty);

            return response;
        }
    }
    
    internal class DeleteWalletGroupCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<DeleteWalletGroupCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(DeleteWalletGroupCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _profileRepository.DeleteGroupWalletAsync(request.GroupId, request.UserId);

            if (response.Status)
                await _domainEvent.WalletDeletedEvent(request.GroupId, "GroupAction");

            return response;
        }
    }
}
