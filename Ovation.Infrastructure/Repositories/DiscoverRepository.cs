using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;

namespace Ovation.Persistence.Repositories
{
    internal class DiscoverRepository(IServiceScopeFactory serviceScopeFactory) : BaseRepository<User>(serviceScopeFactory), IDiscoverRepository
    {
        public async Task<ResponseData> GetBlueChipHoldersAsync(int page, string userPath, Guid userId)
        {
            try
            {
                var response = new ResponseData();

                var data = await _context.UserBadges
                    .Where(_ => _.BadgeName.Contains("BlueChip") && !userPath.Equals("all", StringComparison.OrdinalIgnoreCase)
                    ? _.User.UserPath.Path.Name == userPath : _.User.UserPath.Path.Name != null)
                    .Include(_ => _.User)
                    .ThenInclude(_ => _.UserStat)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .OrderBy(p => p.CreatedDate)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.User.Username,
                        x.User.Email,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.CoverImage,
                        x.User.UserProfile.Location,
                        x.User.UserProfile.Bio,
                        x.User.UserStat.TotalNft,
                        x.User.UserStat.BadgeEarned,
                        PathName = x.User.UserPath.Path.Name,
                        x.BadgeName,
                        x.User.UserStat.Followers,
                        x.User.UserStat.Following,
                        isVerified = x.User.VerifiedUsers.Select(v => v.Type).ToList(),
                        bluechipImage = x.User.UserBlueChips.Select(b => new
                        {
                            b.Bluechip.ImageUrl, b.Bluechip.CollectionName
                        }).ToList()
                    })
                    .ToListAsync();

                response.Status = true;
                response.Message = "Data Fetched";
                response.RankData = (userId != Guid.Empty && data?.Count > 0) ? await GetUserRanking("BluechipCount", userId) : null;
                response.Data = data;
                return response;
            }
            catch (Exception _)
            {
                return new();
            }
        }

        public async Task<ResponseData> GetFeaturedProfilesAsync(Guid userId)
        {
            try
            {
                var response = new ResponseData();

                response.Data = await _context.Users                    
                    .Include(_ => _.UserProfile)
                    .Include(_ => _.UserStat)
                    .Where(_ => _.Active == 1 
                    &&  _.UserProfile.ProfileImage != null && _.UserProfile.CoverImage != null
                    && _.UserProfile.ProfileImage != "" && _.UserProfile.CoverImage != ""
                    && _.UserProfile.Bio != null && _.UserStat.TotalNft > 0)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .OrderByDescending(o => o.CreatedDate)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.Username,
                        x.UserProfile.DisplayName,
                        x.UserProfile.ProfileImage,
                        x.UserProfile.CoverImage,                       
                        x.UserProfile.Bio,                       
                        PathName = x.UserPath.Path.Name,
                        IsFollowing = x.UserFollowerUsers.Where(_ => _.FollowerId == userId.ToByteArray()).FirstOrDefault() != null ? true : false,
                        isVerified = x.VerifiedUsers.Select(v => v.Type).ToList()
                    })
                    .FirstOrDefaultAsync();

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

        public async Task<ResponseData> GetFounderNftAsync(int page, string userPath, Guid userId)
        {
            try
            {
                var response = new ResponseData();

                var data = await _context.UserStats
                    .Where(_ => _.FounderNft > 0 && !userPath.Equals("all", StringComparison.OrdinalIgnoreCase)
                    ? _.User.UserPath.Path.Name == userPath : _.User.UserPath.Path.Name != null)
                    .OrderByDescending(_ => _.FounderNft)
                    .ThenBy(_ => _.CreatedDate)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.User.Username,
                        x.User.Email,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.CoverImage,
                        x.User.UserProfile.Location,
                        x.User.UserProfile.Bio,
                        x.FounderNft,
                        PathName = x.User.UserPath.Path.Name,
                        x.BadgeEarned,
                        x.Followers,
                        x.Following,
                        isVerified = x.User.VerifiedUsers.Select(v => v.Type).ToList()
                    })
                    .ToListAsync();

                response.Status = true;
                response.Message = "Data Fetched";
                response.RankData = (userId != Guid.Empty && data?.Count > 0) ? await GetUserRanking("FounderNft", userId) : null;
                response.Data = data;
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetHighestNetWorth(int page, string userPath, Guid userId)
        {
            try
            {
                var response = new ResponseData();

                var data = await _context.UserStats
                    .Where(_ => _.Networth > 0 && !userPath.Equals("all", StringComparison.OrdinalIgnoreCase)
                    ? _.User.UserPath.Path.Name == userPath : _.User.UserPath.Path.Name != null)
                    .OrderByDescending(p => p.Networth)
                    .ThenBy(_ => _.CreatedDate)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.User.Username,
                        x.User.Email,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.CoverImage,
                        x.User.UserProfile.Location,
                        x.User.UserProfile.Bio,
                        x.TotalNft,
                        x.Networth,
                        PathName = x.User.UserPath.Path.Name,
                        x.BadgeEarned,
                        x.Followers,
                        x.Following,
                        isVerified = x.User.VerifiedUsers.Select(v => v.Type).ToList()
                    })
                    .ToListAsync();

                response.Status = true;
                response.Message = "Data Fetched";
                response.Data = data;
                response.RankData = (userId != Guid.Empty && data?.Count > 0) ? await GetUserRanking("Networth", userId) : null;
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetHighestValuedNftAsync(int page, string userPath, Guid userId)
        {
            try
            {
                var response = new ResponseData();
                var exchange = Constant._chainsToValue;

                var data = await _context.UserHighestNfts
                    .Where(n => n.Usd > 0 && !userPath.Equals("all", StringComparison.OrdinalIgnoreCase)
                    ? n.User.UserPath.Path.Name == userPath : n.User.UserPath.Path.Name != null)
                    .OrderByDescending(_ => _.Usd)
                    .ThenByDescending(_ => _.CreatedDate)
                    .ThenByDescending(x => x.UpdatedDate)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.User.Username,
                        x.User.Email,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.CoverImage,
                        x.User.UserProfile.Location,
                        x.User.UserProfile.Bio,
                        PathName = x.User.UserPath.Path.Name,
                        x.Name,
                        x.ImageUrl,
                        x.NftId,
                        Chain = !string.IsNullOrEmpty(x.TradeSymbol) ? x.TradeSymbol : x.Chain,
                        x.Worth,
                        x.Usd, //(!string.IsNullOrEmpty(x.TradeSymbol)) ? (x.Worth * exchange[x.TradeSymbol!.Trim().ToLower()]).Value.ToString("F2") : (x.Worth * exchange[x.Chain!.Trim().ToLower()]).Value.ToString("F2"),
                        Followers = (x.User.UserStat.Xfollowers != null) ? x.User.UserStat.Xfollowers : 0,
                        Following = (x.User.UserStat.Xfollowing != null) ? x.User.UserStat.Xfollowing : 0,
                        x.CreatedDate,
                        x.UpdatedDate,
                        isVerified = x.User.VerifiedUsers.Select(v => v.Type).ToList()
                    })
                    .ToListAsync();

                response.Data = data;
                response.Status = true;
                response.Message = "Data Fetched";
                response.RankData = (userId != Guid.Empty && data?.Count > 0) ? await GetUserTopNfthRanking(userId) : null;
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetMostFollowedAsync(int page, Guid userId)
        {
            try
            {
                var response = new ResponseData();

                response.Data = await GetMostFollowedByPathAsync(page, userId);

                response.Status = true;
                response.Message = "Data Fetched";
                response.RankData = (userId != Guid.Empty) ? await GetUserRanking("XFollowers", userId) : null;
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetMostSalesAsync(int page, string userPath, Guid userId)
        {
            try
            {
                var response = new ResponseData();

                var data = await _context.UserStats
                    .Where(_ => _.SoldNftsValue > 0 && !userPath.Equals("all", StringComparison.OrdinalIgnoreCase) 
                    ? _.User.UserPath.Path.Name == userPath : _.User.UserPath.Path.Name != null)
                    .OrderByDescending(p => p.SoldNftsValue)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.User.Username,
                        x.User.Email,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.CoverImage,
                        x.User.UserProfile.Location,
                        x.User.UserProfile.Bio,
                        x.Views,
                        x.Followers,
                        x.Following,
                        PathName = x.User.UserPath.Path.Name,
                        isVerified = x.User.VerifiedUsers.Select(v => v.Type).ToList(),
                        x.SoldNftsValue
                    })
                    .ToListAsync();

                response.Status = true;
                response.Message = "Data Fetched";
                response.Data = data;
                response.RankData = (userId != Guid.Empty && data?.Count > 0) ? await GetUserRanking("SoldNftsValue", userId) : null;
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetMostSoldAsync(int page, string userPath, Guid userId)
        {
            try
            {
                var response = new ResponseData();

                var data = await _context.UserStats
                    .Where(_ => _.SoldNftsTotal > 0 && !userPath.Equals("all", StringComparison.OrdinalIgnoreCase)
                    ? _.User.UserPath.Path.Name == userPath : _.User.UserPath.Path.Name != null)
                    .OrderByDescending(p => p.SoldNftsTotal)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.User.Username,
                        x.User.Email,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.CoverImage,
                        x.User.UserProfile.Location,
                        x.User.UserProfile.Bio,
                        x.Views,
                        x.Followers,
                        x.Following,
                        PathName = x.User.UserPath.Path.Name,
                        isVerified = x.User.VerifiedUsers.Select(v => v.Type).ToList(),
                        x.SoldNftsTotal
                    })
                    .ToListAsync();

                response.Status = true;
                response.Message = "Data Fetched";
                response.Data = data;
                response.RankData = (userId != Guid.Empty && data.Count > 0) ? await GetUserRanking("SoldNftsTotal", userId) : null;
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetMostViewedAsync(int page, Guid userId)
        {
            try
            {
                var response = new ResponseData();

                response.Data = await _context.UserStats
                    .Where(_ => _.Views > 0)
                    .OrderByDescending(p => p.Views)
                    .ThenBy(_ => _.CreatedDate)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.User.Username,
                        x.User.Email,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.CoverImage,
                        x.User.UserProfile.Location,
                        x.User.UserProfile.Bio,
                        x.Views,
                        x.BadgeEarned,
                        PathName = x.User.UserPath.Path.Name,
                        IsFollowing = x.User.UserFollowerUsers.Where(_ => _.FollowerId == userId.ToByteArray()).FirstOrDefault() != null ? true : false,
                        x.Followers,
                        x.Following,
                        isVerified = x.User.VerifiedUsers.Select(v => v.Type).ToList()
                    })
                    .ToListAsync();

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

        public async Task<ResponseData> GetNftRankingAsync(int page, string userPath)
        {
            try
            {
                var response = new ResponseData();

                response.Data = new
                {
                    NmberOfNfts = await GetTopNftHoldersAsync(page, userPath, Guid.Empty),
                    HighestValuedNfts = await GetHighestValuedNftAsync(page, userPath, Guid.Empty)
                };

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

        public async Task<ResponseData> GetTopContributorsAsync(int page)
        {
            try
            {
                var response = new ResponseData();

                response.Data = await _context.Users
                    .Include(_ => _.UserProfile)
                    .Include(_ => _.UserStat)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.Username,
                        x.Email,
                        x.UserProfile.DisplayName,
                        x.UserProfile.ProfileImage,
                        x.UserProfile.CoverImage,
                        x.UserProfile.Location,
                        x.UserProfile.Bio,
                        x.UserStat.TotalNft,
                        x.UserStat.BadgeEarned,
                        PathName = x.UserPath.Path.Name,
                        Experiences = x.UserExperiences.Count(),
                        x.UserStat.Followers,
                        x.UserStat.Following,
                        isVerified = x.VerifiedUsers.Select(v => v.Type).ToList()
                    })
                    .Where(_ => _.Experiences > 0)
                    .OrderByDescending(p => p.Experiences)
                    .ToListAsync();

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

        public async Task<ResponseData> GetTopCreatorsAsync(int page, string userPath)
        {
            try
            {
                var response = new ResponseData();

                response.Data = await _context.UserStats
                    .Where(_ => _.NftCreated > 0 && !userPath.Equals("all", StringComparison.OrdinalIgnoreCase)
                    ? _.User.UserPath.Path.Name == userPath : true)
                    .OrderByDescending(p => p.NftCreated)
                    .ThenBy(_ => _.CreatedDate)
                    .AsNoTracking()
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.User.Username,
                        x.User.Email,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.CoverImage,
                        x.User.UserProfile.Location,
                        x.User.UserProfile.Bio,
                        x.NftCreated,
                        PathName = x.User.UserPath.Path.Name,
                        x.BadgeEarned,
                        x.Followers,
                        x.Following,
                        isVerified = x.User.VerifiedUsers.Select(v => v.Type).ToList()
                    })
                    .ToListAsync();

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

        public async Task<ResponseData> GetTopNftHoldersAsync(int page, string userPath, Guid userId)
        {
            try
            {
                var response = new ResponseData();

                response.Data = await _context.UserStats
                    .Where(_ => _.TotalNft > 0 && !userPath.Equals("all", StringComparison.OrdinalIgnoreCase)
                    ? _.User.UserPath.Path.Name == userPath : _.User.UserPath.Path.Name != null)
                    .AsNoTracking()
                    .OrderByDescending(p => p.TotalNft)
                    .ThenBy(_ => _.CreatedDate)
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.User.Username,
                        x.User.Email,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.CoverImage,
                        x.User.UserProfile.Location,
                        x.User.UserProfile.Bio,
                        PathName = x.User.UserPath.Path.Name,
                        x.TotalNft,
                        x.BadgeEarned,
                        x.Followers,
                        x.Following,
                        isVerified = x.User.VerifiedUsers.Select(v => v.Type).ToList()
                    })
                    .ToListAsync();

                response.Status = true;
                response.Message = "Data Fetched";
                response.RankData = (userId != Guid.Empty) ? await GetUserRanking("TotalNft", userId) : null;
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }
        
        public async Task<RankingDto?> GetUserTopNfthRanking(Guid userId)
        {
            try
            {
                var rankingQuery = await _context.Set<RankingDto>()
                .FromSqlRaw($@"WITH ranked_users AS (
                        SELECT
                            UserId,
                            Usd as user_value,
                            RANK() OVER (ORDER BY Usd DESC) AS rank_position
                        FROM user_highest_nfts
                    )
                    SELECT 
                        ru.rank_position as Ranking,
                        CAST(ru.user_value AS CHAR) as Value,
                        (SELECT COUNT(*) FROM user_stats) AS UserCount
                    FROM ranked_users ru
                    WHERE ru.UserId = @userId
                    ", new MySqlParameter("@userId", userId.ToByteArray()))
                .AsNoTracking()
                .FirstOrDefaultAsync();

                if(rankingQuery == null)
                {
                    rankingQuery = new RankingDto();
                    rankingQuery.UserCount = await _context.Users.CountAsync();
                }                    

                return rankingQuery;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return null;
            }
        }

        public async Task<RankingDto?> GetUserRanking(string column, Guid userId)
        {
            try
            {
                var rankingQuery = await _context.Set<RankingDto>()
                .FromSqlRaw($@"WITH ranked_users AS (
                        SELECT
                            UserId,
                            {column} as user_value,
                            RANK() OVER (ORDER BY {column} DESC) AS rank_position
                        FROM user_stats
                    )
                    SELECT 
                        ru.rank_position as Ranking,
                        CAST(ru.user_value AS CHAR) as Value,
                        (SELECT COUNT(*) FROM user_stats) AS UserCount
                    FROM ranked_users ru
                    WHERE ru.UserId = @userId
                    ", new MySqlParameter("@userId", userId.ToByteArray()))
                .AsNoTracking()
                .FirstOrDefaultAsync();

                return rankingQuery;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return null;
            }
        }

        
        private async Task<object> GetAllMostFollowedAsync(int page, Guid userId)
        {
            try
            {
                return await _context.UserStats
                    .Where(_ => _.Followers > 0)
                    .OrderByDescending(p => p.Followers)
                    .ThenBy(_ => _.Views)
                    .AsNoTracking()
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.User.Username,
                        x.User.Email,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.CoverImage,
                        x.User.UserProfile.Location,
                        x.User.UserProfile.Bio,
                        x.Views,
                        x.Followers,
                        x.Following,
                        PathName = x.User.UserPath.Path.Name,
                        IsFollowing = x.User.UserFollowerUsers.Where(_ => _.FollowerId == userId.ToByteArray()).FirstOrDefault() != null ? true : false,
                        isVerified = x.User.VerifiedUsers.Select(v => v.Type).ToList()
                    })
                    .ToListAsync();
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        private async Task<object> GetMostFollowedByPathAsync(int page, Guid userId)
        {
            try
            {
                return await _context.UserStats
                    .Where(_ => _.Xfollowers > 0)
                    .OrderByDescending(p => p.Xfollowers)
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.UserId),
                        x.User.Username,
                        x.User.Email,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.CoverImage,
                        x.User.UserProfile.Location,
                        x.User.UserProfile.Bio,
                        x.Views,
                        Followers = x.Xfollowers,
                        Following = x.Xfollowing,
                        PathName = x.User.UserPath.Path.Name,
                        //IsFollowing = x.User.UserFollowerUsers.Where(_ => _.FollowerId == userId.ToByteArray()).FirstOrDefault() != null ? true : false,
                        isVerified = x.User.VerifiedUsers.Select(v => v.Type).ToList()
                    })
                    .ToListAsync();
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }        
    }
}
