using Ovation.Application.DTOs;
using Ovation.Domain.Entities;

namespace Ovation.Application.Repositories
{
    public interface IAssetRepository : IBaseRepository<NftCollectionsDatum>
    {
        Task<ResponseData> GetCollectionAsync(int collectionId);
        Task<ResponseData> GetCollectionTokensAsync(int collectionId, TokenQueryParametersDto parameters);
        Task<ResponseData> GetCollectionOwnerDistributionAsync(int collectionId);
        Task<ResponseData> GetTokenAsync(int tokenId);
        Task<ResponseData> GetTokenTransactionActivitiesAsync(int tokenId);
        Task GetUserNfts(Guid userId, string address, string chain, Guid? walletId = null);
        Task GetUserTransactions(Guid userId, string address, string chain, Guid? walletId = null);
    }
}
