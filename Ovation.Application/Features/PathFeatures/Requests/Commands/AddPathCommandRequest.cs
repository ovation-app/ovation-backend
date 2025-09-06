using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.PathFeatures.Requests.Commands
{
    public sealed record AddPathCommandRequest(PathDto PathDto) : IRequest<ResponseData>;
}
