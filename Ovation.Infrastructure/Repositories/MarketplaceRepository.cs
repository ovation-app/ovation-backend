using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;


namespace Ovation.Persistence.Repositories
{
    internal class MarketplaceRepository : BaseRepository<UserNftSale>, IMarketplaceRepository
    {
        public MarketplaceRepository(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory) { }

        public async Task<ResponseData> GetMarketPlaceDataAsync(
            string? cursor = null,
            int pageSize = 10,
            string? sortDirection = null)
        {
            var response = new ResponseData();

            int cursorInt = 0;
            if (!string.IsNullOrEmpty(cursor))
                cursorInt = DecodeBase64ToInteger(cursor);

            try
            {
                var data = await _context.UserNftSales
                    .Where(_ => cursorInt != 0 ? _.Id < cursorInt : _.Id > cursorInt)
                    .Include(_ => _.Nft)
                    .Include(_ => _.User)
                    .AsSplitQuery()
                    .OrderByDescending(p => p.Id)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        x.Id,
                        x.Metadata,
                        x.SaleCreatedDate,
                        x.SalePrice,
                        x.SaleCurrency,
                        x.SaleUrl,
                        x.NftId,
                        NftData = new
                        {
                            x.Nft.Id,
                            x.Nft.Name,
                            x.Nft.Description,
                            x.Nft.ImageUrl,
                            x.Nft.CollectionId,
                            x.Nft.Chain,
                            CollectionName = x.Nft.Collection.ContractName,
                            CollectionImg = x.Nft.Collection.LogoUrl,
                            CollectionAddress = x.Nft.Collection.ContractAddress,
                        },
                        User = new
                        {
                            UserId = new Guid(x.User.UserId),
                            x.User.Username,
                            x.User.UserProfile.DisplayName,
                            x.User.UserProfile.ProfileImage,
                        },
                    })
                    .ToListAsync();

                if (data != null && data.Count == pageSize)
                    response.Cursor = EncodeIntegerToBase64(data.Last().Id);

                response.Data = data;

                response.Status = true;

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }
    }
}
