using MediatR;
using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.AuthFeatures.Requests.Commands
{
    public sealed record ChangePasswordCommandRequest(ChangePasswordDto ChangePassword) : IRequest<ResponseData>;
}
