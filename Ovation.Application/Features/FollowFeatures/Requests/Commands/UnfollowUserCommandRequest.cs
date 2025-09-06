using MediatR;
using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.FollowFeatures.Requests.Commands
{
    public sealed record UnfollowUserCommandRequest(Guid UserId, Guid FollowerId) : IRequest<ResponseData>;
}
