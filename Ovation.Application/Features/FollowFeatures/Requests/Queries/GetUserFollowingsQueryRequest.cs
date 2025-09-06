using MediatR;
using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.FollowFeatures.Requests.Queries
{
    public sealed record GetUserFollowingsQueryRequest(Guid UserId, int Page, Guid AuthUser) : IRequest<ResponseData>;
}
