using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands;

public sealed record UpdateNftSaleCommandRequest(
    Guid UserId, 
    long NftId, 
    UpdateNftSaleDto SaleDto
) : IRequest<ResponseData>; 