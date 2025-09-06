using Ovation.Application.DTOs;

namespace Ovation.Application.Repositories
{
    public interface IDiscoverRepository
    {
        Task<ResponseData> GetTopNftHoldersAsync(int page, string userPath, Guid userId);

        Task<RankingDto?> GetUserRanking(string column, Guid userId);

        Task<ResponseData> GetHighestValuedNftAsync(int page, string userPath, Guid userId);

        Task<RankingDto?> GetUserTopNfthRanking(Guid userId);

        Task<ResponseData> GetBlueChipHoldersAsync(int page, string userPath, Guid userId);

        Task<ResponseData> GetHighestNetWorth(int page, string userPath, Guid userId);

        Task<ResponseData> GetTopContributorsAsync(int page);

        Task<ResponseData> GetFounderNftAsync(int page, string userPath, Guid userId);

        Task<ResponseData> GetTopCreatorsAsync(int page, string userPath);

        Task<ResponseData> GetMostViewedAsync(int page, Guid userId);

        Task<ResponseData> GetMostFollowedAsync(int page, Guid userId);

        Task<ResponseData> GetNftRankingAsync(int page, string userPath);

        Task<ResponseData> GetMostSoldAsync(int page, string userPath, Guid userId);

        Task<ResponseData> GetMostSalesAsync(int page, string userPath, Guid userId);

        Task<ResponseData> GetFeaturedProfilesAsync(Guid userId);
    }
}
