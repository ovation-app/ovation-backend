using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Newtonsoft.Json;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Enums;
using Ovation.Application.DTOs.Enums.Badges;
using Ovation.Application.DTOs.Enums.Badges.ProfileComplete.Milestones;
using Ovation.Application.DTOs.SignUp;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services;
using Ovation.Persistence.Services.Clients;
using Ovation.Persistence.Services.Clients.NFTScan;

namespace Ovation.Persistence.Repositories
{
    internal class ProfileRepository(IServiceScopeFactory serviceScopeFactory) : BaseRepository<UserProfile>(serviceScopeFactory), IProfileRepository
    {
        public async Task HandleReferralAsync(string referral, Guid userId)
        {
            var affilation = await _context.UserAffilations
                .FirstOrDefaultAsync(_ => _.Code == referral);

            if (affilation != null)
            {
                var userReferral = new UserReferral
                {
                    InviteeId = userId.ToByteArray(),
                    InviterId = affilation.UserId
                };

                affilation.Invited += 1;
                await _context.UserReferrals.AddAsync(userReferral);

                var notification = new List<NotificationDto>();

                switch (affilation.Invited)
                {
                    case 5:
                        if (!await IsBadgeEarnedAsync(BadgeStruct.UserInvites5, affilation.UserId))
                        {
                            AddNotification(BadgeStruct.UserInvites5, new Guid(affilation.UserId), ref notification);
                        }
                        break;

                    case 10:
                        if (!await IsBadgeEarnedAsync(BadgeStruct.UserInvites10, affilation.UserId))
                        {
                            AddNotification(BadgeStruct.UserInvites10, new Guid(affilation.UserId), ref notification);
                        }
                        break;

                    case 25:
                        if (!await IsBadgeEarnedAsync(BadgeStruct.UserInvites25, affilation.UserId))
                        {
                            AddNotification(BadgeStruct.UserInvites25, new Guid(affilation.UserId), ref notification);
                        }
                        break;

                    case 50:
                        if (!await IsBadgeEarnedAsync(BadgeStruct.UserInvites50, affilation.UserId))
                        {
                            AddNotification(BadgeStruct.UserInvites50, new Guid(affilation.UserId), ref notification);
                        }
                        break;

                    default:
                        break;
                }

                await _unitOfWork.SaveChangesAsync();

                await SaveNotificationAsync(notification);
            }
        }

        public async Task CheckAlphaLaunchAndTesterBadge(Guid userId)
        {
            var isQualifyPrivateTester = await IsUserQualifiedForPrivateTester(userId);

            var isQualifyAlphaLaunch = await IsUserQualifiedForAlphaLaunch(userId);

            if (!isQualifyPrivateTester && !isQualifyAlphaLaunch) return;

            var isTesterBadgeEarned = await IsBadgeEarnedAsync(BadgeStruct.AlphaTester, userId);

            var isAlphaLaunhBadgeEarned = await IsBadgeEarnedAsync(BadgeStruct.AlphaLaunch, userId);

            if (isTesterBadgeEarned && isAlphaLaunhBadgeEarned) return;

            var notification = new List<NotificationDto>();

            if (!isTesterBadgeEarned)
            {
                AddNotification(BadgeStruct.AlphaTester, userId, ref notification);
            }

            if (!isAlphaLaunhBadgeEarned)
            {
                AddNotification(BadgeStruct.AlphaLaunch, userId, ref notification);
            }

            await SaveNotificationAsync(notification);
        }

        public async Task<ResponseData> ViewProfileAsync(Guid userId, Guid viewerId)
        {
            if (userId == Guid.Empty || viewerId == Guid.Empty)
                return new();

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var isUserViewed = await _context.UserProfileViews
                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.ViewerId == viewerId.ToByteArray());

                if (isUserViewed != null) return new ResponseData { Message = "User have viewed profile before!" };

                var profileView = new UserProfileView
                {
                    UserId = userId.ToByteArray(),
                    ViewerId = viewerId.ToByteArray()
                };

                var userStat = await _context.UserStats
                    .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                if (userStat == null) return new ResponseData { Message = "User not found" };

                userStat.Views += 1;

                await _context.UserProfileViews.AddAsync(profileView);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Message = "Profile view record saved!", Status = true };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> GetAuthUserAsync(Guid userId)
        {
            var response = new ResponseData();
            
            try
            {
                response.Data = await _context.Users
                    .Include(_ => _.UserProfile)
                    .Include(_ => _.UserStat)
                    .Include(_ => _.UserSocial)
                    .Include(_ => _.VerifiedUsers)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(_ => _.UserId == userId.ToByteArray() && _.Active == active)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.Username,
                        x.Email,
                        x.GoogleId,
                        x.CreatedDate,
                        isVerified = x.VerifiedUsers.Select(v => v.Type).ToList(),
                        Verifications = x.VerifiedUsers.Select(v => new { v.Type, v.Handle, }).ToList(),
                        Paths = new
                        {
                            Name = x.UserPath != null && x.UserPath.Path != null ? x.UserPath.Path.Name : null,
                            Description = x.UserPath != null && x.UserPath.Path != null ? x.UserPath.Path.Description : null,
                            PathId = x.UserPath != null && x.UserPath.PathId != null ? new Guid(x.UserPath.PathId) : Guid.Empty
                        },

                        Profile = x.UserProfile != null ? new
                        {
                            x.UserProfile.DisplayName,
                            x.UserProfile.BirthDate,
                            x.UserProfile.Location,
                            x.UserProfile.Bio,
                            x.UserProfile.CoverImage,
                            x.UserProfile.ProfileImage,
                        } : null,

                        Socials = x.UserSocial.Socials != null ? JsonConvert.DeserializeObject<List<SocialsDto>>(x.UserSocial.Socials) : null,

                        UserStats = x.UserStat != null ? new
                        {
                            x.UserStat.NftCreated,
                            x.UserStat.BadgeEarned,
                            Followers = x.UserStat.Xfollowers,
                            Following = x.UserStat.Xfollowing,
                            x.UserStat.Networth,
                            x.UserStat.NftCollected,
                            x.UserStat.Views,
                            x.UserStat.TotalNft,
                            Invited = x.UserAffilation != null ? x.UserAffilation.Invited : 0,
                        } : null
                    })
                    .SingleOrDefaultAsync();

                response.Status = true;

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserAsync(string username, Guid userId)
        {
            var response = new ResponseData();

            try
            {
                response.Data = await _context.Users
                    .Include(_ => _.UserProfile)
                    .Include(_ => _.UserStat)
                    .Include(_ => _.UserSocial)
                    .Include(_ => _.VerifiedUsers)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(_ => _.Username == username && _.Active == active)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.Username,
                        x.Email,
                        x.CreatedDate,
                        IsFollowing = x.UserFollowerUsers.Where(_ => _.FollowerId == userId.ToByteArray()).FirstOrDefault() != null ? true : false,
                        isVerified = x.VerifiedUsers.Select(v => v.Type).ToList(),
                        Verifications = x.VerifiedUsers.Select(v => new { v.Type, v.Handle, }).ToList(),
                        Profile = x.UserProfile != null ? new
                        {
                            x.UserProfile.DisplayName,
                            x.UserProfile.BirthDate,
                            x.UserProfile.Location,
                            x.UserProfile.Bio,
                            x.UserProfile.CoverImage,
                            x.UserProfile.ProfileImage,
                        } : null,

                        Socials = x.UserSocial.Socials != null ? JsonConvert.DeserializeObject<List<SocialsDto>>(x.UserSocial.Socials) : null,

                        Paths = new
                        {
                            Name = x.UserPath != null && x.UserPath.Path != null ? x.UserPath.Path.Name : null,
                            Description = x.UserPath != null && x.UserPath.Path != null ? x.UserPath.Path.Description : null,
                            PathId = x.UserPath != null && x.UserPath.PathId != null ? new Guid(x.UserPath.PathId) : Guid.Empty
                        },

                        UserStats = x.UserStat != null ? new
                        {
                            x.UserStat.NftCreated,
                            x.UserStat.BadgeEarned,
                            Followers = x.UserStat.Xfollowers,
                            Following = x.UserStat.Xfollowing,
                            x.UserStat.Networth,
                            x.UserStat.NftCollected,
                            x.UserStat.Views,
                            x.UserStat.TotalNft,
                            Invited = x.UserAffilation != null ? x.UserAffilation.Invited : 0,
                        } : null
                    })
                    .SingleOrDefaultAsync();

                response.Status = true;

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserExperienceAsync(Guid userId, int page)
        {
            var response = new ResponseData();

            try
            {
                response.Data = await _context.UserExperiences
                    .Where(_ => _.UserId == userId.ToByteArray())
                    .OrderBy(p => p.CreatedDate)
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        Id = new Guid(x.Id),
                        x.Skill,
                        x.Role,
                        x.Department,
                        x.Company,
                        x.StartDate,
                        x.EndDate,
                        x.Description
                    })
                    .ToListAsync();

                response.Status = true;

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserBadgesAsync(Guid userId, int page)
        {
            var response = new ResponseData();

            try
            {
                response.Data = await _context.UserBadges
                    .Where(_ => _.UserId == userId.ToByteArray() && _.Active == 1 && _.BadgeNameNavigation.Active == active)
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .GroupBy(_ => _.BadgeName)
                    .Select(x => new
                    {
                        x.First().EarnedAt,
                        BadgeId = new Guid(x.First().BadgeNameNavigation.BadgeId),
                        x.First().BadgeName,
                        x.First().BadgeNameNavigation.Description,
                        x.First().BadgeNameNavigation.ImageUrl
                    })
                    .ToListAsync();

                response.Status = true;

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserSocialsAsync(Guid userId)
        {
            var response = new ResponseData();

            try
            {
                response.Data = await _context.UserSocials
                    .Where(_ => _.UserId == userId.ToByteArray())
                    .Select(x => new
                    {
                        Socials = x.Socials != null ? JsonConvert.DeserializeObject<List<SocialsDto>>(x.Socials) : null
                    })
                    .FirstOrDefaultAsync();

                response.Status = true;

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserStatsAsync(Guid userId)
        {
            var response = new ResponseData();

            try
            {
                response.Data = await _context.UserStats
                    .Where(_ => _.UserId == userId.ToByteArray())
                    .Select(x => new
                    {
                        x.Networth,
                        Followers = x.Xfollowers,
                        x.BadgeEarned,
                        Following = x.Xfollowing,
                        x.NftCollected,
                        x.NftCreated,
                        x.FounderNft,
                        x.NftCollections,
                        x.SoldNftsValue,
                        x.SoldNftsTotal,
                        x.TotalNft,
                        x.Views,
                        Invited = x.User.UserAffilation != null ? x.User.UserAffilation.Invited : 0,
                        HighestNft = x.User.UserHighestNfts.Select(_ => new
                        {
                            _.Name,
                            _.ImageUrl,
                            _.Chain,
                            _.Worth,
                            _.Usd,
                        }).FirstOrDefault(),

                        Maxi = GetMaxi(userId),
                        HasHiddenNft = HasHiddenNft(userId)
                    })
                    .FirstOrDefaultAsync();

                response.Status = true;

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        private string GetMaxi(Guid userId)
        {
            var data = _context.UserNftData
                .Where(_ => _.UserId == userId.ToByteArray())
                .GroupBy(nft => nft.Chain)
                .Select(group => new
                {
                    Blockchain = group.Key,
                    NftCount = group.Count()
                })
                .OrderByDescending(x => x.NftCount)
                .Take(1)
                .FirstOrDefault();
            if (data == null || string.IsNullOrEmpty(data?.Blockchain)) return string.Empty;

            return (data.Blockchain.Equals("eth", StringComparison.OrdinalIgnoreCase)) ? "Ethereum" : data.Blockchain;
        }

        private bool HasHiddenNft(Guid userId)
        {
            var data = _context.UserNftData
                .Where(_ => _.UserId == userId.ToByteArray() && _.Public == inactive)
                .Count();

            return data > 0 ? true : false;
        }

        public async Task<ResponseData> GetUserWalletsAsync(Guid userId)
        {
            var response = new ResponseData();

            try
            {
                response.Data = await _context.UserWalletGroups
                    .Where(_ => _.UserId == userId.ToByteArray() && _.UserWallets.Count > 0)
                    .Take(perPage)
                    .Select(x => new
                    {
                        GroupId = new Guid(x.Id),

                        WalletType = x.WalletId != null ? new
                        {
                            TypeId = new Guid(x.WalletId),
                            WalletTypeName = x.Wallet.Name,
                            x.Wallet.LogoUrl,
                        } : null,

                        Wallets = x.UserWallets.Select(w => new
                        {
                            Id = new Guid(w.Id),
                            w.WalletAddress,
                            w.Chain,      
                            w.NftCount,
                            Usd = w.NftsValue,
                            MultiChain = !string.IsNullOrEmpty(w.MetaData)
                            ? JsonConvert.DeserializeObject<UserWalletMetaData?>(w.MetaData).MultiChains
                            : w.NftCount > 0 ? new List<string> { w.Chain!} : null
                        }).ToList()                   
                    })
                    .ToListAsync();

                response.Status = true;
                response.Message = response.Data != null ? "Data Fetched" : "No wallet found";

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetWalletsAsync(Guid userId)
        {
            var response = new ResponseData();

            try
            {
                response.Data = await _context.UserWallets
                    .Where(_ => _.UserId == userId.ToByteArray())
                    .Take(perPage)
                    .Select(x => new
                    {
                        Id = new Guid(x.Id),
                        x.WalletAddress,
                        x.Chain,
                        WalletType = x.WalletId != null ? new
                        {
                            TypeId = new Guid(x.WalletId),
                            WalletTypeName = x.Wallet.Name,
                            x.Wallet.LogoUrl,
                        } : null,
                        MultiChain = !string.IsNullOrEmpty(x.MetaData) ? JsonConvert.DeserializeObject<UserWalletMetaData?>(x.MetaData).MultiChains : null
                    })
                    .ToListAsync();

                response.Status = true;
                response.Message = response.Data != null ? "Data Fetched" : "No wallet found";

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserNftsAsync(Guid userId, Guid authUser, NftQueryParametersDto parameters)
        {
            try
            {
                long? idCursor = null; decimal? priceCursor = null;
                if (!string.IsNullOrEmpty(parameters.Next))
                    (idCursor, priceCursor) = DecodeFromBase64(parameters.Next);

                var response = authUser != Guid.Empty && userId == authUser ? await GetAuthUserNftsAsync(userId, parameters, idCursor, priceCursor)
                    : await GetUserNfts(userId, parameters, idCursor, priceCursor);

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        private async Task<ResponseData> GetAuthUserNftsAsync(Guid userId, NftQueryParametersDto parameters, long? idCursor = null, decimal? priceCursor = null)
        {
            try
            {
                var response = new ResponseData();

                var chain = HandleParameter(parameters.Chain);
                var created = HandleCreatedParameter(parameters.Created);
                var isPrivate = HandleParameter(parameters.Private);
                var forSale = HandleParameter(parameters.ForSale);
                var (wallet, param) = await HandleParameter(parameters.Wallet);

                param.Add(new MySqlParameter("@p0", created));
                param.Add(new MySqlParameter("@p1", isPrivate));
                param.Add(new MySqlParameter("@p2", chain));
                param.Add(new MySqlParameter("@p3", wallet != null ? "hasWallet" : null));
                param.Add(new MySqlParameter("@p4", idCursor));
                param.Add(new MySqlParameter("@p5", priceCursor));
                param.Add(new MySqlParameter("@p6", userId.ToByteArray()));
                param.Add(new MySqlParameter("@p7", perPage));
                param.Add(new MySqlParameter("@p8", forSale));
                param.Add(new MySqlParameter("@s", Constant.Solana));
                param.Add(new MySqlParameter("@t", Constant.Tezos));

                if(string.IsNullOrEmpty(wallet))
                    wallet = "n.UserWalletId is not null";

                var data = await _context.Set<NftDataDto>()
                    .FromSqlRaw($@"
                        SET @created = @p0;
                        SET @isPrivate = @p1;
                        SET @chain = @p2;
                        SET @wallet = @p3;
                        SET @id = @p4;
                        SET @usd = @p5;
                        SET @forSale = @p8;

                        SELECT 
                            n.id,
                            n.name,
                            n.Description,
                            n.ImageUrl,
                            c.ContractName,
                            n.Chain As Type,
                            c.ParentCollection AS CollectionId,
                            n.animationurl,
                            n.CustodyDate,
                            n.TokenId,
                            CASE WHEN n.public = 1 THEN FALSE ELSE TRUE END AS isPrivate,
                            CASE WHEN n.Favorite = 1 THEN TRUE ELSE FALSE END AS isFav,
                            n.ForSale,
                            s.SalePrice,
                            s.SaleCurrency,
                            s.SaleUrl,
                            s.Metadata AS SaleMetadata,
                            CASE
                                WHEN n.Chain = @s OR n.Chain = @t THEN n.LastTradePrice
                                ELSE c.FloorPrice
                            END AS native,
                            JSON_ARRAY(JSON_OBJECT('Currency',
                                            'USD',
                                            'Value',
                                            ROUND(COALESCE(CASE
                                                                WHEN n.Chain = @s OR n.Chain = @t THEN n.LastTradePrice * r.usd_rate
                                                                ELSE c.FloorPrice * r.usd_rate
                                                            END,
                                                            0),
                                                    2)),
                                    JSON_OBJECT('Currency',
                                            CASE WHEN n.Chain = 'abstract' OR n.Chain = 'base' THEN 'eth' ELSE n.Chain END,
                                            'Value',
                                            COALESCE(CASE
                                                        WHEN n.Chain = @s OR n.Chain = @t THEN n.LastTradePrice
                                                        ELSE c.FloorPrice
                                                    END,
                                                    0))) AS prices,
                            ROUND(COALESCE(CASE
                                                WHEN n.Chain = @s OR n.Chain = @t THEN n.LastTradePrice * r.usd_rate
                                                ELSE c.FloorPrice * r.usd_rate
                                            END,
                                            0),
                                    2) AS usd
                        FROM
                            user_nft_data n
                                JOIN
                            chain_rates r ON n.Chain = r.symbol
                                LEFT JOIN
                            user_nft_collection_data c ON n.CollectionId = c.id
                                LEFT JOIN
                            user_nft_sales s ON n.Id = s.NftId
                        WHERE
                            n.UserId = @p6
                            AND (@isPrivate is null or n.Public = @isPrivate) AND (@created is null or n.Created = @created)
                            AND (@chain is null or n.Chain = @chain)
                            AND (@forSale is null or n.ForSale = @forSale)
                            AND (@wallet is null or {wallet})
                            AND CASE
                                WHEN @id is null AND @usd is null THEN
                                (ROUND(COALESCE(CASE
                                            WHEN n.Chain = @s OR n.Chain = @t THEN n.LastTradePrice * r.usd_rate
                                            ELSE c.FloorPrice * r.usd_rate
                                        END,
                                        0),
                                2) >= 0)
                                ELSE
                                (ROUND(
                                   COALESCE(
                                     CASE 
                                       WHEN n.Chain = @s or n.Chain = @t THEN n.LastTradePrice * r.usd_rate
                                       ELSE c.FloorPrice * r.usd_rate
                                     END,
                                     0
                                   ),
                                     2
                                   ) <= @usd  AND n.id < @id
                                 ) END
                        ORDER BY usd DESC , n.id DESC
                        LIMIT @p7;
                    ", param.ToArray())
                    .AsNoTracking()
                    .ToListAsync();

                if (data != null && data.Count == perPage)
                    response.Cursor = EncodeToBase64(data.Last().Id, data.Last().usd);

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

        private async Task<ResponseData> GetUserNfts(Guid userId, NftQueryParametersDto parameters, long? idCursor = null, decimal? priceCursor = null)
        {            
            try
            {
                var response = new ResponseData();

                var chain = HandleParameter(parameters.Chain);
                var created = HandleCreatedParameter(parameters.Created);
                var forSale = HandleParameter(parameters.ForSale);
                //var wallet = await HandleParameter(parameters.Wallet);

                var data = await _context.Set<NftDataDto>()
                    .FromSqlInterpolated($@"
                        SET @created = {created};
                        SET @chain = {chain};
                        SET @forSale = {forSale};

                        SET @id = {idCursor};
                        SET @usd = {priceCursor};

                        SELECT 
                            n.id,
                            n.name,
                            n.Description,
                            n.ImageUrl,
                            c.ContractName,
                            n.Chain As Type,
                            c.ParentCollection AS CollectionId,
                            n.animationurl,
                            n.CustodyDate,
                            n.TokenId,
                            CASE WHEN n.public = 1 THEN FALSE ELSE TRUE END AS isPrivate,
                            CASE WHEN n.Favorite = 1 THEN TRUE ELSE FALSE END AS isFav,
                            n.ForSale,
                            s.SalePrice,
                            s.SaleCurrency,
                            s.SaleUrl,
                            s.Metadata AS SaleMetadata,
                            CASE
                                WHEN n.Chain = 'solana' OR n.Chain = 'tezos' THEN n.LastTradePrice
                                ELSE c.FloorPrice
                            END AS native,
                            JSON_ARRAY(JSON_OBJECT('Currency',
                                            'USD',
                                            'Value',
                                            ROUND(COALESCE(CASE
                                                                WHEN n.Chain = 'solana' OR n.Chain = 'tezos' THEN n.LastTradePrice * r.usd_rate
                                                                ELSE c.FloorPrice * r.usd_rate
                                                            END,
                                                            0),
                                                    2)),
                                    JSON_OBJECT('Currency',
                                            CASE WHEN n.Chain = 'abstract' OR n.Chain = 'base' THEN 'eth' ELSE n.Chain END,
                                            'Value',
                                            COALESCE(CASE
                                                        WHEN n.Chain = 'solana' OR n.Chain = 'tezos' THEN n.LastTradePrice
                                                        ELSE c.FloorPrice
                                                    END,
                                                    0))) AS prices,
                            ROUND(COALESCE(CASE
                                                WHEN n.Chain = 'solana' OR n.Chain = 'tezos' THEN n.LastTradePrice * r.usd_rate
                                                ELSE c.FloorPrice * r.usd_rate
                                            END,
                                            0),
                                    2) AS usd
                        FROM
                            user_nft_data n
                                JOIN
                            chain_rates r ON n.Chain = r.symbol
                                LEFT JOIN
                            user_nft_collection_data c ON n.CollectionId = c.id
                                LEFT JOIN
                            user_nft_sales s ON n.Id = s.NftId
                        WHERE
                            n.UserId = {userId.ToByteArray()}
                            AND n.Public = 1 AND (@created is null or n.Created = @created)
                            AND (@chain is null or n.Chain = @chain)
                            AND (@forSale is null or n.ForSale = @forSale)
                            
                            AND CASE
                                WHEN @id is null AND @usd is null THEN
                                (ROUND(COALESCE(CASE
                                            WHEN n.Chain = 'solana' OR n.Chain = 'tezos' THEN n.LastTradePrice * r.usd_rate
                                            ELSE c.FloorPrice * r.usd_rate
                                        END,
                                        0),
                                2) >= 0)
                                ELSE
                                (ROUND(
                                   COALESCE(
                                     CASE 
                                       WHEN n.Chain = 'solana' or n.Chain = 'tezos' THEN n.LastTradePrice * r.usd_rate
                                       ELSE c.FloorPrice * r.usd_rate
                                     END,
                                     0
                                   ),
                                     2
                                   ) <= @usd  AND n.id < @id
                                 ) END
                        ORDER BY usd DESC , n.id DESC
                        LIMIT {perPage};
                    ")
                    .AsNoTracking()
                    .ToListAsync();

                if (data != null && data.Count == perPage)
                    response.Cursor = EncodeToBase64(data.Last().Id, data.Last().usd);                    

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

        public async Task<ResponseData> GetUserTopNftsAsync(int limit, Guid userId)
        {
            try
            {
                var response = new ResponseData();

                var data = await GetUserTopNft(limit, userId);

                response.Data = data;
                response.Status = true;

                return response;
            }
            catch (Exception _)
            {
                return new();
            }
        }

        private async Task<List<NftDataDto>?> GetUserTopNft(int limit, Guid userId)
        {
            try
            {
                var topNftQuery = await _context.Set<NftDataDto>()
                .FromSqlRaw($@"SELECT 
                        n.id,
                        n.name,
                        n.Description,
                        n.ImageUrl,
                        n.Chain AS Type,
                        c.ContractName,
                        c.ParentCollection AS CollectionId,
                        n.animationurl,
                        n.CustodyDate,
                        n.TokenId,
                        n.ForSale,
                        s.SalePrice,
                        s.SaleCurrency,
                        s.SaleUrl,
                        s.Metadata AS SaleMetadata,
                        CASE WHEN n.public = 1 THEN FALSE ELSE TRUE END AS isPrivate,
                        CASE WHEN n.Favorite = 0 THEN FALSE ELSE TRUE END AS isFav,
                        CASE
                            WHEN n.Chain = 'solana' OR n.Chain = 'tezos' THEN n.LastTradePrice
                            ELSE c.FloorPrice
                        END AS native,
                        JSON_ARRAY(JSON_OBJECT('Currency',
                                        'USD',
                                        'Value',
                                        ROUND(COALESCE(CASE
                                                            WHEN n.Chain = 'solana' OR n.Chain = 'tezos' THEN n.LastTradePrice * r.usd_rate
                                                            ELSE c.FloorPrice * r.usd_rate
                                                        END,
                                                        0),
                                                2)),
                                JSON_OBJECT('Currency',
                                        CASE WHEN n.Chain = 'abstract' OR n.Chain = 'base' THEN 'eth' ELSE n.Chain END,
                                        'Value',
                                        COALESCE(CASE
                                                    WHEN n.Chain = 'solana' OR n.Chain = 'tezos' THEN n.LastTradePrice
                                                    ELSE c.FloorPrice
                                                END,
                                                0))
                        ) AS prices,
                        ROUND(COALESCE(CASE
                                    WHEN n.Chain = 'solana' OR n.Chain = 'tezos' THEN n.LastTradePrice * r.usd_rate
                                    ELSE c.FloorPrice * r.usd_rate
                                END,
                                0),
                        2) AS usd
                    FROM
                        user_nft_data n
                            JOIN
                        chain_rates r ON n.Chain = r.symbol
                            LEFT JOIN
                        user_nft_collection_data c ON n.CollectionId = c.id
                            LEFT JOIN
                        user_nft_sales s ON n.Id = s.NftId
                    WHERE n.UserId = @userId and Public = 1
                    ORDER BY 
                      usd DESC
                      limit {limit};
                    ", new MySqlParameter("@userId", userId.ToByteArray()))
                .AsNoTracking()
                .ToListAsync();

                if (topNftQuery == null)
                    return null;
                
                return topNftQuery;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return null;
            }
        }

        private int? HandleParameter(bool? param)
        {
            if (param == null)
                return null;
            else if (param.Value)
                return 0;
            else if (!param.Value)
                return 1;
            else
                return null;
        }

        private int? HandleCreatedParameter(bool? param)
        {
            if (param == null)
                return null;
            else if (param.Value)
                return 1;
            else if (!param.Value)
                return 0;
            else
                return null;
        }

        private string? HandleParameter(string? param)
        {
            if (string.IsNullOrEmpty(param))
                return null;
            else
                return param;
        }

        private async Task<(string?, List<object>)> HandleParameter(Guid? param)
        {
            if (param == null || param.Value == Guid.Empty)
                return (null, new());
            else
            {
                var wallets = await _context.UserWallets
                    .Where(w => w.WalletGroupId == param.Value.ToByteArray())
                    .Select(w => w.Id)
                    .ToListAsync();

                static string ToSqlHex(byte[] bytes) => "X'" + BitConverter.ToString(bytes).Replace("-", "") + "'";

                var sqlConditions = new List<string>();
                var parameters = new List<object>();

                //var dataa = string.Join(",", wallets.Select(ToSqlHex));

                for (int i = 0; i < wallets.Count; i++)
                {
                    var pName = $"@w{i}";
                    sqlConditions.Add($"n.UserWalletId = {pName}");
                    parameters.Add(new MySqlParameter(pName, wallets[i]));
                }

                var data = string.Join(" OR ", sqlConditions);

                return (data, parameters);
            }
                
        }

        public async Task<ResponseData> GetTokenAsync(int assetId, Guid userId)
        {
            try
            {
                var response = new ResponseData();
                response.Data = await _context.UserNftData
                    .Where(n => n.Id == assetId && n.UserId == userId.ToByteArray())
                    .Include(c => c.Collection)
                    .Include(s => s.UserNftSales)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.ImageUrl,
                        x.AnimationUrl,
                        LastTradePrice = x.Chain == Constant.Solana || x.Chain == Constant.Tezos ? x.LastTradePrice : x.Collection.FloorPrice,
                        Usd = CalculatePrice(new Trade(x.LastTradeSymbol, x.LastTradePrice, x.Collection.FloorPrice, x.Chain)),
                        x.Id,
                        x.Chain,
                        x.Description,
                        x.ContractAddress,
                        x.TokenId,
                        x.TokenAddress,
                        x.Name,
                        x.Collection.ContractName,
                        CollectionId = x.Collection.ParentCollection,
                        Website = EF.Functions.JsonExtract<string?>(x.Collection.ParentCollectionNavigation.MetaData, "$.Website"),
                        TokenStandard = EF.Functions.JsonExtract<string?>(x.Collection.ParentCollectionNavigation.MetaData, "$.ErcType"),
                        Royalty = EF.Functions.JsonExtract<string?>(x.Collection.ParentCollectionNavigation.MetaData, "$.Royalty.ValueKind"),
                        x.CustodyDate,
                        ForSale = x.ForSale == 1 ? true : false,
                        SaleData = x.UserNftSales.Select(s => new
                        {
                            s.SalePrice,
                            s.SaleCurrency,
                            s.SaleUrl,
                            s.Metadata,
                            s.SaleCreatedDate,
                            s.SaleUpdatedDate
                        }).FirstOrDefault(),
                        Users = new
                        {
                            x.User.UserProfile.ProfileImage,
                            x.User.Username,
                            x.User.UserProfile.DisplayName
                        }
                    }).FirstOrDefaultAsync();

                response.Status = true;
                response.Message = "Data Fetched";
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetTokenTransactionActivitiesAsync(int tokenId)
        {
            var nft = await _context.UserNftData.Where(_ => _.Id == (long)tokenId)
                .AsNoTracking()
                .Select(x => new
                {
                    x.Chain,
                    x.ContractAddress,
                    x.TokenId,
                    x.TokenAddress
                }).FirstOrDefaultAsync();

            if (nft == null) return new ResponseData { StatusCode = 404, Message = "NFT not found" };

            var response = new ResponseData();

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            switch (nft.Chain?.ToLower())
            {
                case Constant.Solana:

                    var solanaService = scope.ServiceProvider.GetRequiredService<SolanaService>();
                    response.Data = await solanaService.GetNftTransactionAsync(nft.TokenAddress, Constant.Solana);
                    break;

                case Constant.Archway:

                    //var archwayService = scope.ServiceProvider.GetRequiredService<ArchwayService>();
                    //await archwayService.GetUserNftsAsync(address, userId);
                    break;
                case Constant.Cosmos:

                    break;
                case Constant.Tezos:

                    var tezosService = scope.ServiceProvider.GetRequiredService<TezosService>();
                    response.Data = await tezosService.GetNftTransactionAsync(nft.ContractAddress, nft.TokenId);
                    break;
                case Constant.Ton:

                    var tonService = scope.ServiceProvider.GetRequiredService<TonService>();
                    response.Data = await tonService.GetNftTransactionAsync(nft.TokenAddress, Constant.Ton);
                    break;

                default:
                    var evmService = scope.ServiceProvider.GetRequiredService<EvmsService>();
                    response.Data = await evmService.GetNftTransactionAsync(nft.ContractAddress, nft.TokenId, nft.Chain);

                    break;
            }

            response.Status = true;
            response.Message = "Data Fetched";
            return response;
        }

        public async Task<ResponseData> GetFavNftsAsync(Guid userId)
        {
            //var exchange = Constant._chainsToValue;
            //var exchange2 = Constant._chainsToValueFloor;
            try
            {
                var response = new ResponseData();
                response.Data = await _context.UserNftData
                .Where(_ => _.UserId == userId.ToByteArray() && _.Favorite == 1)
                .AsNoTracking()
                .OrderBy(_ => _.Id)
                .Take(3)
                .Select(x => new
                {
                    x.Id,
                    x.ContractAddress,
                    x.TokenAddress,
                    x.TokenId,
                    x.Description,
                    x.Name,
                    CollectionName = x.Collection.ContractName,
                    CollectionId = x.Collection.ParentCollection,
                    x.Chain,
                    x.ImageUrl,
                    isPrivate = x.Public == 0 ? true : false,
                    NativePrices = x.FloorPrice,
                    USD = CalculatePrice(new Trade(x.LastTradeSymbol, x.LastTradePrice, x.Collection.FloorPrice, x.Chain)),
                    ForSale = x.ForSale == 1 ? true : false,
                    SaleData = x.UserNftSales.Select(s => new
                    {
                        s.SalePrice,
                        s.SaleCurrency,
                        s.SaleUrl,
                        s.Metadata,
                        s.SaleCreatedDate,
                        s.SaleUpdatedDate
                    }).FirstOrDefault()
                })
                .ToListAsync();

                response.Status = true;
                response.Message = response.Data != null ? "Data Fetched" : "No data not found";

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserCollectionsAsync(Guid userId, Guid authUser, NftQueryParametersDto parameters)
        {
            var response = new ResponseData();

            int cursor = 0;
            if (!string.IsNullOrEmpty(parameters.Next))
                cursor = DecodeBase64ToInteger(parameters.Next);

            try
            {
                var exchange = Constant._chainsToValue;

                var data = await _context.UserNftCollectionData
                    .Where(_ => _.UserId == userId.ToByteArray() && (cursor != 0 ? _.Id < cursor : _.Id > cursor)
                    && (!string.IsNullOrEmpty(parameters.Chain) ? _.Chain == parameters.Chain : true)
                    && (parameters.Wallet != null ? _.UserWalletId == parameters.Wallet.Value.ToByteArray() : true))
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(perPage)
                    .Select(x => new
                    {
                        Id = x.ParentCollection,
                        ignore = x.Id,
                        x.LogoUrl,
                        x.OwnsTotal,
                        x.ItemTotal,
                        x.FloorPrice,
                        x.ContractName,
                        x.Chain,
                    })
                    .ToListAsync();

                if (data != null && data.Count == perPage)
                    response.Cursor = EncodeIntegerToBase64(data.Last().ignore);

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

        public async Task<ResponseData> GetUserVerificationDataAsync(Guid userId)
        {
            var response = new ResponseData();

            try
            {
                response.Data = await _context.VerifiedUsers
                    .Where(_ => _.UserId == userId.ToByteArray())
                    .Select(x => new
                    {
                        x.Id,
                        x.Handle,
                        VerificationType = x.Type
                    })
                    .ToListAsync();

                response.Status = true;
                response.Message = response.Data != null ? "Data Fetched" : "User verification data not found";

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserNftOverviewAsync(Guid userId)
        {
            var response = new ResponseData();

            try
            {
                response.Data = await _context.UserNftData
                    .Where(_ => _.UserId == userId.ToByteArray() && _.Public == 1)
                    .GroupBy(nft => nft.Chain)
                    .Select(group => new
                    {
                        Blockchain = group.Key,
                        NftCount = group.Count()
                    })
                    .OrderByDescending(x => x.NftCount)
                    .ToListAsync();

                response.Status = true;
                response.Message = response.Data != null ? "Data Fetched" : "No data not found";

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserTransactionsDataAsync(Guid userId, string? next = null)
        {
            DateTime? cursor = null;
            if (!string.IsNullOrEmpty(next))
                cursor = DecodeBase64ToDateTime(next);

            var response = new ResponseData();
            var perPage = 15;
            try
            {
                //var exchange = Constant._chainsToValue;

                var data = await _context.UserNftTransactions
                    .Where(_ => cursor == null ? _.UserId == userId.ToByteArray() : _.UserId == userId.ToByteArray() && _.TranxDate < cursor)
                    .OrderByDescending(o => o.TranxDate)
                    .Take(perPage)
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.ContractName,
                        x.Image,
                        x.EventType,
                        x.From, 
                        x.To,
                        Price = CalculatePrice(new Trade(x.TradeSymbol, x.TradePrice, x.TradePrice, x.Chain)),
                        x.TranxDate
                    })
                    .ToListAsync();

                if (data != null && data.Count == perPage)
                    response.Cursor = EncodeDateTimeToBase64(data.Last().TranxDate);

                response.Data = data;

                response.Status = true;
                response.Message = response.Data != null ? "Data Fetched" : "No data not found";

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserPortfolioRecordAsync(Guid userId)
        {
            var response = new ResponseData();

            try
            {
                var exchange = Constant._chainsToValue;

                var data = await _context.UserPortfolioRecords
                    .Where(u => u.UserId == userId.ToByteArray())
                    .GroupBy(u => new { u.UserId, Month = u.CreatedDate.Value.Year * 100 + u.CreatedDate.Value.Month })
                    .Select(g => g.OrderByDescending(u => u.CreatedDate)
                    .Select(u => new
                    {
                        u.CreatedDate.Value.Date,
                        Value = u.UsdValue,
                        //Data = new
                        //{
                            //Value = u.UsdValue,
                        //    //AltValue = (!string.IsNullOrEmpty(u.Chain) && u.Chain != "all")? 
                        //    //Math.Round(double.Parse(u.Value, NumberStyles.Float, CultureInfo.InvariantCulture) * (double)exchange[u.Chain!.Trim().ToLower()], 2).ToString("F2") : null
                        //}
                    })
                    .FirstOrDefault())                    
                    .ToListAsync();

                var portfolioData = new List<PortfolioData>();

                if(data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        portfolioData.Add(new PortfolioData { Date = item.Date, Value = item.Value.Value });
                    }
                    response.Data = FillMissingMonthsV2(portfolioData);
                }

                response.Status = true;
                response.Message = response.Data != null ? "Data Fetched" : "No data not found";

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        private static List<PortfolioData> FillMissingMonths(List<PortfolioData> existingData)
        {
            if (existingData == null || existingData.Count == 0)
                return new List<PortfolioData>();

            // Sort the data by date
            existingData = existingData.OrderBy(d => d.Date).ToList();

            var filledData = new List<PortfolioData>();
            DateTime startDate = existingData.First().Date;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            //Console.WriteLine(currentDate.ToString());

            var lastKnownValue = existingData.First().Value;
            int dataIndex = 0;

            for (DateTime date = startDate; date < currentDate.AddMonths(1); date = date.AddMonths(1))
            {
                // Move to the next known data point if available
                if (dataIndex < existingData.Count && existingData[dataIndex].Date.Year == date.Year && existingData[dataIndex].Date.Month == date.Month)
                {
                    lastKnownValue = existingData[dataIndex].Value;
                    filledData.Add(existingData[dataIndex]);
                    dataIndex++;
                }
                else
                {
                    // Add missing month with last known value
                    filledData.Add(new PortfolioData { Date = date, Value = lastKnownValue });
                }
            }

            return filledData;
        }

        private static List<PortfolioData> FillMissingMonthsV2(List<PortfolioData> existingData)
        {
            if (existingData == null || existingData.Count == 0)
                return new List<PortfolioData>();

            // Sort the data by date
            existingData = existingData.OrderBy(d => d.Date).ToList();

            var filledData = new List<PortfolioData>();
            DateTime startDate = existingData.First().Date;
            DateTime startOfYear = new DateTime(2025, 1, 1);
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            if(startDate > startOfYear)
            {
                // Step 1: Add zero-value months from January to the first data point
                for (DateTime date = startOfYear; date < startDate; date = date.AddMonths(1))
                {
                    var dd = existingData.FirstOrDefault(_ => _.Date.Year == date.Year && _.Date.Month == date.Month);
                    if (dd != null)
                        continue;
                    filledData.Add(new PortfolioData { Date = date, Value = 0 });
                }
            }
            

            // Step 2: Fill in data from first entry up to current month
            var lastKnownValue = existingData.First().Value;
            int dataIndex = 0;

            for (DateTime date = startDate; date < currentDate.AddMonths(1); date = date.AddMonths(1))
            {
                if (dataIndex < existingData.Count &&
                    existingData[dataIndex].Date.Year == date.Year &&
                    existingData[dataIndex].Date.Month == date.Month)
                {
                    lastKnownValue = existingData[dataIndex].Value;
                    filledData.Add(existingData[dataIndex]);
                    dataIndex++;
                }
                else
                {
                    filledData.Add(new PortfolioData { Date = date, Value = lastKnownValue });
                }
            }

            return filledData;
        }

        public async Task<ResponseData> AddUserExperienceAsync(UserExperienceDto userExperience, Guid userId)
        {
            DateOnly? startDate = null;
            DateOnly? endDate = null;

            if (!string.IsNullOrEmpty(userExperience.StartDate))
            {
                if (DateOnly.TryParse(userExperience.StartDate, out DateOnly dateOnly))
                    startDate = dateOnly;
                else
                    return new ResponseData { Message = "Invalid date for startDate field" };
            }
            if (!string.IsNullOrEmpty(userExperience.EndDate))
            {
                if (DateOnly.TryParse(userExperience.EndDate, out DateOnly endD))
                    endDate = endD;
                else
                    return new ResponseData { Message = "Invalid date for endDate field" };
            }
            var response = new ResponseData();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var expId = Guid.NewGuid();
                var exp = new UserExperience
                {
                    Id = expId.ToByteArray(),
                    Company = userExperience.Company,
                    Department = userExperience.Department,
                    Description = userExperience.Description,
                    EndDate = endDate,
                    Role = userExperience.Role,
                    Skill = userExperience.Skill,
                    StartDate = startDate,
                    UserId = userId.ToByteArray()
                };
                await _context.UserExperiences.AddAsync(exp);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                response.Status = true;
                response.Message = "Experience Added Successfully";
                response.GuidValue = expId;

                await _domainEvent.TaskPerformedEvent(userId, ProfileCompleteMilestones.AddExperience.ToString());

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> AddWalletAsync(WalletAcct walletAcct, Guid userId)
        {
            _sentryService.AddTag("add.wallet", userId.ToString());

            var crumbsData = new Dictionary<string, string> ();

            crumbsData.Add("WalletType", walletAcct.WalletTypeId.ToString());
            int idx = 1;
            foreach (var item in walletAcct.Data)
            {
                crumbsData.Add($"Address{idx}", item.WalletAddress);
                crumbsData.Add($"Chain{idx}", item.Chain);
                idx++;
            }

            _sentryService.AddBreadcrumb("Add new wallet started", "add.wallet", crumbsData);

            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var assetRepository = scope.ServiceProvider.GetRequiredService<IAssetRepository>();            

                //foreach (var item in walletAcct.Data)
                //{
                //    if (await userRepository.FindWalletAsync(item.WalletAddress, item.Chain, userId))
                //        return new ResponseData { StatusCode = 409, Message = "Wallet address already exist" };
                //}

            if (walletAcct.WalletTypeId != null)
            {
                _sentryService.AddBreadcrumb("Wallet Type validation started", "wallet.type.validation");
                var verifyWalletType = await _context.Wallets.FirstOrDefaultAsync(p => p.Id == walletAcct.WalletTypeId.Value.ToByteArray());

                if (verifyWalletType == null)
                {
                    _sentryService.AddBreadcrumb("Wallet Type does not exist", "wallet.type.validation");
                    return new ResponseData { Message = "Wallet Type does not exist" };
                }
                    
            }
            await _unitOfWork.BeginTransactionAsync();


            try
            {
                _sentryService.AddBreadcrumb("Wallet group preparation started", "wallet.group");

                var id = walletAcct.WalletTypeId != null ? walletAcct.WalletTypeId.Value.ToByteArray() : null;

                var group = await _context.UserWalletGroups
                    .IgnoreAutoIncludes().AsNoTracking()
                    .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.WalletId == id);

                var groupId = Guid.NewGuid().ToByteArray();

                if (group == null)
                {
                    _sentryService.AddBreadcrumb("User don't have an existing wallet group", "wallet.group");
                    var walletGroup = new UserWalletGroup
                    {
                        Id = groupId,
                        WalletId = id,
                        UserId = userId.ToByteArray()
                    };

                    await _context.UserWalletGroups.AddAsync(walletGroup);
                    _sentryService.AddBreadcrumb("New wallet group added", "wallet.group");
                }
                else
                    groupId = group.Id;

                var response = new ResponseData { Status = true, Message = string.Empty };

                _sentryService.AddBreadcrumb("Wallet addresses validation started", "wallet.address");

                var addresses = walletAcct.Data.Select(_ => _.WalletAddress.ToLower()).ToArray();

                var exitingWallets = await _context.UserWallets
                    .Where(u => addresses.Contains(u.WalletAddress))
                    .AsNoTracking()
                    .ToListAsync();

                var toRemove = new List<AddressData>();

                var count = 0;
                var isAuto = walletAcct.WalletTypeId != null;

                foreach (var item in walletAcct.Data)
                {
                    var wallets = exitingWallets.FindAll(_ => _.WalletAddress.ToLower() == item.WalletAddress.ToLower()).ToList();

                    if(userRepository.ScreenWallet(wallets, item.Chain, userId, isAuto))
                    {
                        var wallet = new UserWallet
                        {
                            Id = Guid.NewGuid().ToByteArray(),
                            UserId = userId.ToByteArray(),
                            WalletAddress = item.WalletAddress.ToLower(),
                            WalletId = id,
                            NftCount = 0,
                            Chain = item.Chain.ToLower(),
                            WalletGroupId = groupId
                        };
                        await _context.UserWallets.AddAsync(wallet);

                        _sentryService.AddBreadcrumb("Wallet address added", "wallet.address",
                            new Dictionary<string, string> { { "Address", item.WalletAddress }, { "Chain", item.Chain} });
                        count +=1;
                    }
                    else
                    {
                        response.Message += $"Wallet address: {HelperFunctions.ShrinkWalletAddress(item.WalletAddress)} already exists \n";

                        _sentryService.AddBreadcrumb("Wallet address already exists", "wallet.address",
                            new Dictionary<string, string> { { "Address", item.WalletAddress }, { "Chain", item.Chain } });

                        toRemove.Add(item);
                    }
                    
                }                

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _sentryService.AddBreadcrumb("Changes commited", "wallet.added");

                response.Message += $"{count} wallet(s) was added successfully";
                response.Status = count > 0 ? true : false;

                foreach (var item in toRemove)
                {
                    walletAcct.Data.Remove(item);
                }

                await _domainEvent.TaskPerformedEvent(userId, ProfileCompleteMilestones.ConnectWallet.ToString());

                foreach (var item in walletAcct.Data)
                {
                    var chain = item.Chain;

                    await _domainEvent.WalletAddedEvent(userId, item.WalletAddress, chain, walletAcct.WalletTypeId);

                    if (walletAcct.WalletTypeId != null)
                    {
                        await _domainEvent.WalletOwnershipVerifiedEvent(item.WalletAddress);
                    }
                }

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();             
            }
        }

        public async Task<ResponseData> VerifyUserAsync(VerifiyUserDTO verifiyUser, Guid userId)
        {
            var isUserVerified = await _context.VerifiedUsers
                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.Type == verifiyUser.VerificationType);

            if (isUserVerified != null)
                return new ResponseData { Message = $"User already verified with the {isUserVerified.Type} username: {isUserVerified.Handle}!" };

            var isExist = await _context.VerifiedUsers
                .FirstOrDefaultAsync(_ => _.Handle == verifiyUser.Handle && _.Type == verifiyUser.VerificationType);

            if (isExist != null)
                return new ResponseData { Message = $"A user already verified their account with this {isExist.Handle} handle." };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var verifiedUser = new VerifiedUser
                {
                    Handle = verifiyUser.Handle,
                    Type = verifiyUser.VerificationType,
                    UserId = userId.ToByteArray(),
                    TypeId = verifiyUser.TypeId
                };
                await _context.VerifiedUsers.AddAsync(verifiedUser);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                if(verifiyUser.VerificationType.Equals("x", StringComparison.OrdinalIgnoreCase))
                {
                    await _domainEvent.XAccountConnectedEvent(userId);
                }
                
                return new ResponseData { Status = true, Message = "User Verified!" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> UpdatePersonalInfo(ProfileModDto profileDto, Guid userId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            if (!HelperFunctions.IsValidEmail(profileDto.Email))
                return new ResponseData{ Message = "Invalid Email Address" };

            if (await userRepository.FindEmailAsync(profileDto.Email, userId))
                return new ResponseData { Message = "Invalid Email Address", StatusCode = 409 };

            if (await userRepository.FindUsernameAsync(profileDto.Username, userId))
                return new ResponseData { Message = "Invalid Email Address", StatusCode = 409 };

            DateOnly? dob;

            if (!string.IsNullOrEmpty(profileDto.BirthDate))
            {
                if (DateOnly.TryParse(profileDto.BirthDate, out DateOnly dateOnly))
                    dob = dateOnly;
                else
                    return new ResponseData { Message = "Invalid date for Date of birth field" };
            }
            else
                dob = null;

            if (profileDto.UserPath != null)
            {
                var verifyPath = await _context.PathTypes.FirstOrDefaultAsync(p => p.Id == profileDto.UserPath.PathId.ToByteArray());

                if (verifyPath == null)
                    return new ResponseData { Message = "Path does not exist" };
            }

            var response = new ResponseData();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var entity = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId.ToByteArray());
                var entity2 = await _context.UserProfiles.SingleOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                if (profileDto.UserPath != null)
                {
                    var entity3 = await _context.UserPaths.SingleOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                    if (entity3 != null)
                    {
                        entity3.PathId = profileDto.UserPath.PathId.ToByteArray();

                        var record = new UserPathRecord { PathId = profileDto.UserPath.PathId.ToByteArray(), UserId = userId.ToByteArray() };

                        await _context.UserPathRecords.AddAsync(record);
                    }
                    else
                        return new ResponseData { StatusCode = 404 };
                }

                if (entity == null || entity2 == null)
                    return new ResponseData { StatusCode = 404 };

                entity.Username = profileDto.Username.Trim();
                entity.Email = profileDto.Email.Trim();

                entity2.DisplayName = profileDto.DisplayName.Trim();
                entity2.Bio = profileDto.Bio;
                entity2.Location = profileDto.Location;
                entity2.BirthDate = dob;
                entity2.ProfileImage = profileDto.ProfileImage;
                entity2.CoverImage = profileDto.CoverImage;

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                response.Status = true;
                response.Message = "Profile details updated";

                if (!string.IsNullOrEmpty(profileDto.Bio))
                    await _domainEvent.TaskPerformedEvent(userId, ProfileCompleteMilestones.FillBio.ToString());
                if (!string.IsNullOrEmpty(profileDto.Location))
                    await _domainEvent.TaskPerformedEvent(userId, ProfileCompleteMilestones.AddLocation.ToString());
                if (!string.IsNullOrEmpty(profileDto.Email))
                    await _domainEvent.TaskPerformedEvent(userId, ProfileCompleteMilestones.AddEmail.ToString());
                if (!string.IsNullOrEmpty(profileDto.Username))
                    await _domainEvent.TaskPerformedEvent(userId, ProfileCompleteMilestones.AddUsername.ToString());

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> UpdateUserPath(UserPathDto userPath, Guid userId)
        {
            if (userPath != null)
            {
                var verifyPath = await _context.PathTypes.FirstOrDefaultAsync(p => p.Id == userPath.PathId.ToByteArray());

                if (verifyPath == null)
                    return new ResponseData { Message = "Path does not exist" };
            }
            else
                return new ResponseData { Message = "Userpath object can't be empty" };

            var response = new ResponseData();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (userPath != null)
                {
                    var entity = await _context.UserPaths.SingleOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                    if (entity != null)
                    {
                        entity.PathId = userPath.PathId.ToByteArray();

                        var record = new UserPathRecord { PathId = userPath.PathId.ToByteArray(), UserId = userId.ToByteArray() };

                        await _context.UserPathRecords.AddAsync(record);
                    }
                    else
                        return new ResponseData { StatusCode = 404 };
                }

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                response.Status = true;
                response.Message = "User Path details updated";

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> UpdateCoverImageAsync(string url, Guid userId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var entity = await _context.UserProfiles.SingleOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                if (entity == null)
                    return new ResponseData { StatusCode = 404 };

                entity.CoverImage = url;

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Status = true, Message = "Cover image updated" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> UpdateSocialsAsync(UserSocialsModDto userSocials, Guid userId)
        {
            if (userSocials == null)
                return new ResponseData { Message = "Nothing was save, no social data received" };

            await _unitOfWork.BeginTransactionAsync();
            var response = new ResponseData();
            try
            {
                var entity = await _context.UserSocials.SingleOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                if (entity == null)
                    return new ResponseData { StatusCode = 404 };

                entity.Socials = JsonConvert.SerializeObject(userSocials.Socials);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                response.Status = true;
                response.Message = "User socials updated";

                await _domainEvent.TaskPerformedEvent(userId, ProfileCompleteMilestones.AddALink.ToString());

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> UpdateExperienceAsync(UserExperienceModDto userExperience, Guid id, Guid userId)
        {
            DateOnly? startDate = null;
            DateOnly? endDate = null;

            if (!string.IsNullOrEmpty(userExperience.StartDate))
            {
                if (DateOnly.TryParse(userExperience.StartDate, out DateOnly dateOnly))
                    startDate = dateOnly;
                else
                    return new ResponseData { Message = "Invalid date for startDate field" };
            }

            if (!string.IsNullOrEmpty(userExperience.EndDate))
            {
                if (DateOnly.TryParse(userExperience.EndDate, out DateOnly endD))
                    endDate = endD;
                else
                    return new ResponseData { Message = "Invalid date for startDate field" };
            }


            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var entity = await _context.UserExperiences.SingleOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.Id == id.ToByteArray());

                if (entity == null)
                    return new ResponseData { StatusCode = 404 };

                entity.StartDate = startDate;
                entity.EndDate = endDate;
                entity.Role = userExperience.Role;
                entity.Company = userExperience.Company;
                entity.Skill = userExperience.Skill;
                entity.Department = userExperience.Department;
                entity.Description = userExperience.Description;

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Status = true, Message = "User experience updated" };

            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> UpdateFavoriteNftAsync(List<IntId> ids, Guid userId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var favNftCount = await _context.UserNftData
                    .Where(_ => _.Favorite == 1 && _.UserId == userId.ToByteArray())
                    .CountAsync();
                
                var newTotal = favNftCount + ids.Count;
                if (favNftCount > 2 || newTotal > 3) return new ResponseData { Message = "Only 3 NFTs can be stored" };

                foreach (var id in ids)
                {                    
                    var nft = await _context.UserNftData
                        .FirstOrDefaultAsync(_ => _.Id == id.Id && _.UserId == userId.ToByteArray());

                    if (nft == null) return new ResponseData { StatusCode = 404, Message = $"NFT with id: {id.Id} not found" };

                    nft.Favorite = 1;

                    _context.UserNftData.Update(nft);
                }

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Status = true, StatusCode = 200, Message = "Updated!!" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> RemoveFavoriteNftAsync(List<IntId> ids, Guid userId)
        {           
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var id in ids)
                {
                    if (id.Id < 1) return new ResponseData { Message = "List contains Invalid Id" };

                    var nft = await _context.UserNftData
                    .FirstOrDefaultAsync(_ => _.Id == id.Id && _.UserId == userId.ToByteArray());

                    if (nft == null) return new ResponseData { StatusCode = 404, Message = "NFT not found" };

                    nft.Favorite = 0;

                    _context.UserNftData.Update(nft);
                }

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Status = true, StatusCode = 200, Message = "Updated!!" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> UpdateFeaturedItemsAsync(IdsWithTypeDto ids, Guid userId)
        {
            return new();

            //await _unitOfWork.BeginTransactionAsync();
            //try
            //{
            //    var entity = await _context.UserFeaturedItems
            //        .Where(_ => _.UserId == userId.ToByteArray())
            //        .FirstOrDefaultAsync();

            //    if (entity == null) return new ResponseData { Message = "Data not found", StatusCode = 404 };

            //    await _unitOfWork.SaveChangesAsync();

            //    await _unitOfWork.CommitTransactionAsync();

            //    return new ResponseData { Status = true, StatusCode = 200, Message = "Updated!!" };
            //}
            //catch (Exception _)
            //{
            //    await _unitOfWork.RollbackAsync();
            //    return new();
            //}
        }

        public async Task<ResponseData> UpdateNftVisibilityAsync(List<NftVisibleDto> nftVisibleDto, Guid userId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach(var item in nftVisibleDto)
                {
                    var entity = await _context.UserNftData
                    .Where(_ => _.UserId == userId.ToByteArray() && _.Id == item.NftId)
                    .FirstOrDefaultAsync();

                    if (entity == null) return new ResponseData { Message = "Data not found", StatusCode = 404 };

                    entity.Public = Convert.ToSByte(item.Public);
                    _context.UserNftData.Update(entity);
                }

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                await _domainEvent.NFTVisibilityChangedEvent(userId);

                return new ResponseData { Status = true, StatusCode = 200, Message = "Updated!!" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<List<FavNfts>?> GetFeatureItemsAsync(Guid userId)
        {
            //FormattableString query = $"select json_object('Id', l.NftId, 'Name', un.Name, 'ImageUrl', un.ImageUrl, 'Description', un.Description, 'Type', un.Type, 'TokenAddress', un.TokenAddress, 'TokenId', un.TokenId) from user_featured_items fn, json_table(fn.Featured, '$[*]' COLUMNS(NftId varchar(36) PATH '$.Id')) as l left join user_nfts un on l.NftId = Concat('0x',hex(un.Id)) where fn.UserId = {userId.ToByteArray()} group by l.NftId, un.Name, un.ImageUrl, un.Description, un.Type, un.TokenAddress, un.TokenId, un.MetaData;";
            //var res = await _context.Database.SqlQuery<string>(query)
            //    .ToListAsync();

            //var favNftList = new List<FavNfts>();

            //if (res != null)
            //{
            //    foreach (var a in res)
            //    {
            //        var nft = JsonConvert.DeserializeObject<FavNfts>(a)!;
            //        nft.Id = HelperFunctions.ConvertHexToGuid(nft.Id!).ToString();
            //        favNftList.Add(nft);
            //    }
            //    return favNftList;
            //}
            return null;
        }

        public async Task<ResponseData> DeleteWalletAsync(Guid id, Guid userId)
        {
            try
            {
                var wallet = await _context.UserWallets
                    .FirstOrDefaultAsync(_ => _.Id == id.ToByteArray() && _.UserId == userId.ToByteArray());

                if (wallet == null) return new ResponseData { StatusCode = 404, Message = "Wallet not found" };                                

                return new ResponseData { Status = true, Message = "Wallet Deletion Request Accepted!" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
            
        }

        public async Task<ResponseData> DeleteGroupWalletAsync(Guid groupId, Guid userId)
        {
            try
            {
                var group = await _context.UserWalletGroups.IgnoreAutoIncludes().FirstOrDefaultAsync(_ => _.Id == groupId.ToByteArray());

                if (group == null) return new ResponseData { StatusCode = 404, Message = "Group not found" };

                var wallets = await _context.UserWallets
                    .Where(_ => _.WalletGroupId == groupId.ToByteArray() && _.UserId == userId.ToByteArray())
                    .ToListAsync();

                if (wallets == null) return new ResponseData { StatusCode = 404, Message = "No Wallet found" };                

                return new ResponseData { Status = true, Message = "Wallet Group Deleted Successfully!" };

            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }

        }

        public async Task<ResponseData> UnlinkUserVerification(string verifyType, Guid userId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var entity = await _context.VerifiedUsers
                    .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.Type == verifyType);

                if (entity == null) return new ResponseData { Message = "User verification not found" };

                _context.VerifiedUsers.Remove(entity);

                if(verifyType.Equals("x", StringComparison.OrdinalIgnoreCase))
                {
                    var stats = await _context.UserStats
                        .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());
                    if(stats != null)
                    {
                        stats.Xfollowers = 0;
                        stats.Xfollowing = 0;
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Status = true, Message = "Unlinked Successfully" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> DeleteExperienceAsync(Guid id, Guid userId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var exp = await _context.UserExperiences
                    .FirstOrDefaultAsync(_ => _.Id == id.ToByteArray());

                if (exp is null) return new ResponseData { StatusCode = 404, Message = "Experience not found" };

                _context.UserExperiences.Remove(exp);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Status = true, Message = "Experience Deleted!" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }


        private async Task<bool> IsTaskDone(string taskName, Guid userId)
        {
            var entity = await _context.UserTasks
                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.TaskName == taskName);

            if (entity == null)
                return false;

            return true;
        }

        private async Task<bool> IsProfileCompleteTasksDone(Guid userId)
        {
            var entity = await _context.UserTasks
                .Where(_ => _.UserId == userId.ToByteArray())
                .ToListAsync();

            if (entity == null)
                return false;

            if (entity.Count > 6) return true;

            else return false;
        }

        private async Task ProfileCompleteBadgeEarned(Guid userId)
        {
            var notification = new List<NotificationDto>
            {
                new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[NotificationReference.Badge],
                    Title = Constant._NotificationTitle[NotificationReference.Badge],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = BadgeStruct.ProfileComplete
                }
            };

            await SaveNotificationAsync(notification);
        }

        private async Task<bool> IsUserQualifiedForPrivateTester(Guid userId)
        {
            var publicLaunch = DateTime.Parse("2024-10-11");

            var entity = await _context.Users
                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.CreatedDate < publicLaunch);

            if (entity == null) return false;

            return true;
        }

        private async Task<bool> IsUserQualifiedForAlphaLaunch(Guid userId)
        {
            var endTime = DateTime.Parse("2025-01-31");

            var entity = await _context.Users
                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.CreatedDate < endTime);

            if (entity == null) return false;

            return true;
        }

        private void AddNotification(string badgeName, Guid userId, ref List<NotificationDto> notification)
        {
            notification.Add(new NotificationDto
            {
                InitiatorId = null,
                ReceiverId = userId,
                Message = Constant._NotificationMessage[NotificationReference.Badge],
                Title = Constant._NotificationTitle[NotificationReference.Badge],
                Reference = NotificationReference.Badge.ToString(),
                ReferenceId = badgeName
            });
        }

        private async Task<bool> IsBadgeEarnedAsync(string badgeName, byte[] userId)
        {
            var isEarned = await _context.UserBadges
                .FirstOrDefaultAsync(_ => _.BadgeName == badgeName && _.UserId == userId);

            if (isEarned == null) return false;

            return true;
        }

        private async Task<bool> IsBadgeEarnedAsync(string badgeName, Guid userId)
        {
            var isEarned = await _context.UserBadges
                .FirstOrDefaultAsync(_ => _.BadgeName == badgeName && _.UserId == userId.ToByteArray());

            if (isEarned == null) return false;

            return true;
        }

        public async Task<ResponseData> UpdateNftSaleAsync(Guid userId, long nftId, UpdateNftSaleDto saleDto)
        {
            try
            {
                var response = new ResponseData();

                // Verify NFT belongs to user
                var nft = await _context.UserNftData
                    .FirstOrDefaultAsync(n => n.Id == nftId && n.UserId == userId.ToByteArray());

                if (nft == null)
                {
                    response.Status = false;
                    response.Message = "NFT not found or does not belong to user";
                    return response;
                }

                // Update the ForSale property in UserNftDatum
                nft.ForSale = (sbyte)(saleDto.ForSale ? 1 : 0);

                // Handle sale metadata
                var existingSale = await _context.UserNftSales
                    .FirstOrDefaultAsync(s => s.NftId == nftId);

                if (saleDto.ForSale)
                {
                    // Validate sale data when marking as for sale
                    if (saleDto.SalePrice <= 0)
                    {
                        response.Status = false;
                        response.Message = "Sale price must be greater than 0";
                        return response;
                    }

                    if (existingSale == null)
                    {
                        // Create new sale record
                        var newSale = new UserNftSale
                        {
                            NftId = nftId,
                            UserId = userId.ToByteArray(),
                            SalePrice = saleDto.SalePrice,
                            SaleCurrency = saleDto.SaleCurrency,
                            SaleUrl = saleDto.SaleUrl,
                            Metadata = saleDto.Metadata != null ? JsonConvert.SerializeObject(saleDto.Metadata) : null,
                            SaleCreatedDate = DateTime.UtcNow,
                            SaleUpdatedDate = DateTime.UtcNow
                        };

                        await _context.UserNftSales.AddAsync(newSale);
                    }
                    else
                    {
                        // Update existing sale record
                        existingSale.SalePrice = saleDto.SalePrice;
                        existingSale.SaleCurrency = saleDto.SaleCurrency;
                        existingSale.SaleUrl = saleDto.SaleUrl;
                        existingSale.Metadata = saleDto.Metadata != null ? JsonConvert.SerializeObject(saleDto.Metadata) : null;
                        existingSale.SaleUpdatedDate = DateTime.UtcNow;
                    }
                }
                else
                {
                    // Remove sale record if exists
                    if (existingSale != null)
                    {
                        _context.UserNftSales.Remove(existingSale);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                response.Status = true;
                response.Message = "NFT sale status updated successfully";
                return response;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return new ResponseData
                {
                    Status = false,
                    Message = "An error occurred while updating NFT sale status"
                };
            }
        }
    }
}
