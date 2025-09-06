using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Repositories
{
    public interface IProfileRepository
    {
        Task HandleReferralAsync(string referral, Guid userId);

        Task CheckAlphaLaunchAndTesterBadge(Guid userId);

        Task<ResponseData> ViewProfileAsync(Guid userId, Guid viewerId);

        Task<ResponseData> GetAuthUserAsync(Guid userId);

        Task<ResponseData> GetUserAsync(string username, Guid userId);

        Task<ResponseData> GetUserExperienceAsync(Guid userId, int page);

        Task<ResponseData> GetUserBadgesAsync(Guid userId, int page);

        Task<ResponseData> GetUserSocialsAsync(Guid userId);

        Task<ResponseData> GetUserStatsAsync(Guid userId);

        Task<ResponseData> GetUserWalletsAsync(Guid userId);

        Task<ResponseData> GetWalletsAsync(Guid userId);

        Task<ResponseData> GetUserNftsAsync(Guid userId, Guid authUser, NftQueryParametersDto parameters);

        Task<ResponseData> GetUserTopNftsAsync(int limit, Guid userId);

        Task<ResponseData> GetTokenAsync(int assetId, Guid userId);

        Task<ResponseData> GetTokenTransactionActivitiesAsync(int tokenId);

        Task<ResponseData> GetUserCollectionsAsync(Guid userId, Guid authUser, NftQueryParametersDto parameters);

        Task<ResponseData> GetUserVerificationDataAsync(Guid userId);

        Task<ResponseData> GetUserNftOverviewAsync(Guid userId);

        Task<ResponseData> GetUserTransactionsDataAsync(Guid userId, string? cursor = null);

        Task<ResponseData> GetUserPortfolioRecordAsync(Guid userId);

        Task<ResponseData> AddUserExperienceAsync(UserExperienceDto userExperience, Guid userId);

        Task<ResponseData> AddWalletAsync(WalletAcct walletAcct, Guid userId);

        Task<ResponseData> VerifyUserAsync(VerifiyUserDTO verifiyUser, Guid userId);

        Task<ResponseData> UpdatePersonalInfo(ProfileModDto profileDto, Guid userId);

        Task<ResponseData> UpdateUserPath(UserPathDto userPath, Guid userId);

        Task<ResponseData> UpdateCoverImageAsync(string url, Guid userId);

        Task<ResponseData> UpdateSocialsAsync(UserSocialsModDto userSocials, Guid userId);

        Task<ResponseData> UpdateExperienceAsync(UserExperienceModDto userExperience, Guid id, Guid userId);

        Task<ResponseData> UpdateFavoriteNftAsync(List<IntId> ids, Guid userId);

        Task<ResponseData> RemoveFavoriteNftAsync(List<IntId> ids, Guid userId);

        Task<ResponseData> UpdateFeaturedItemsAsync(IdsWithTypeDto ids, Guid userId);

        Task<ResponseData> UpdateNftVisibilityAsync(List<NftVisibleDto> nftVisibleDto, Guid userId);

        Task<ResponseData> GetFavNftsAsync(Guid userId);

        Task<List<FavNfts>?> GetFeatureItemsAsync(Guid userId);

        Task<ResponseData> DeleteWalletAsync(Guid id, Guid userId);

        Task<ResponseData> DeleteGroupWalletAsync(Guid groupId, Guid userId);

        Task<ResponseData> UnlinkUserVerification(string verifyType, Guid userId);

        Task<ResponseData> DeleteExperienceAsync(Guid id, Guid userId);

        Task<ResponseData> UpdateNftSaleAsync(Guid userId, long nftId, UpdateNftSaleDto saleDto);

    }
}
