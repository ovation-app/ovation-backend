using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.NewsletterFeatures.Requests.Commands;

namespace Ovation.Application.Features.NewsletterFeatures.Handlers.Commands
{
    internal class AddNewsSubcriberCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<AddNewsSubcriberCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(AddNewsSubcriberCommandRequest request, CancellationToken cancellationToken)
        {
            return await _newsletterRepository.PostSubscriberAsync(request.NewsletterDto);
        }
    }
}
