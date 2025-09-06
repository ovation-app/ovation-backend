using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.BadgeFeatures.Requests.Queries
{
    public sealed record GetBlueChipsQueryRequest(int Page) : IRequest<ResponseData>;
}
