using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.AuthFeatures.Requests.Queries
{
    public sealed record class CheckUsernameQueryRequest(string Username) : IRequest<bool>;
}
