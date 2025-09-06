using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.FeedbackFeatures.Requests.Commands;

namespace Ovation.Application.Features.FeedbackFeatures.Handlers.Commands
{
    internal class AddFeedbackCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<AddFeedbackCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(AddFeedbackCommandRequest request, CancellationToken cancellationToken)
        {
            return await _feedbackRepository.AddUserFeedbackAsync(request.UserFeedback, request.UserId);
        }
    }
}
