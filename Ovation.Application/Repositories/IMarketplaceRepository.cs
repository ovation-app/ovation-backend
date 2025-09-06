using Ovation.Application.DTOs;
using Ovation.Domain.Entities;

namespace Ovation.Application.Repositories
{
    public interface IMarketplaceRepository : IBaseRepository<UserNftSale>
    {
        Task<ResponseData> GetMarketPlaceDataAsync(
            string? cursor = null, 
            int pageSize = 10, 
            string? sortDirection = null);
    }
}
