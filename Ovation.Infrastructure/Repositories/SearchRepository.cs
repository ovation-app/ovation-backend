using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using System.Globalization;

namespace Ovation.Persistence.Repositories
{
    internal class SearchRepository(IServiceScopeFactory serviceScopeFactory) : BaseRepository<User>(serviceScopeFactory), ISearchRepository
    {
        internal new const int perPage = 15;
        public async Task<ResponseData> FindNftAsync(string query, string? cursor, Guid userId)
        {
            try
            {
                var response = new ResponseData();
                query = $"{query.Trim()}%";

                var exchange = Constant._chainsToValue;
                var exchange2 = Constant._chainsToValueFloor;

                var data = await _context.NftsData
                    .Include(x => x.UserNftData)
                    .Where(x => EF.Functions.Like(x.Name, query) || EF.Functions.Like(x.ContractAddress, query)
                     || EF.Functions.Like(x.TokenAddress, query) || EF.Functions.Like(x.TokenId, query))
                    .AsSplitQuery()
                    .OrderBy(_ => _.Id)
                    .Take(perPage)
                    .Select(x => new NftSearchData
                    {
                        Name = x.Name,
                        NftId = x.Id,
                        Chain = x.Type,
                        ImageUrl = x.ImageUrl,
                        AnimationUrl = x.AnimationUrl,
                        ContractName = x.Collection.ContractName,
                        LogoUrl = x.Collection.LogoUrl,
                        ContractAddress = x.ContractAddress,
                        TokenAddress = x.TokenAddress,
                        TokenId = x.TokenId,
                        Description = x.Description,

                        Prices = new List<NftPrice>
                        {
                            new NftPrice
                            {
                                Currency = "USD",
                                Value = CalculatePrice(new Trade(x.LastTradeSymbol, x.LastTradePrice, x.Collection.FloorPrice, x.Type))
                            },
                            new NftPrice
                            {
                                Currency = x.Type == Constant.Archway ? "USDC" : x.Type == Constant.Base ? "ETH" : x.Type,
                                Value = (x.Collection.FloorPrice ?? x.LastTradePrice) != null ? 
                                decimal.Parse(x.Collection.FloorPrice ?? x.LastTradePrice, NumberStyles.Float, CultureInfo.InvariantCulture).ToString("G29")
                                : "0"
                            }
                        },

                        Users = x.UserNftData.Select(u => new NftUser
                        {
                            Username = u.User.Username,
                            UserId = new Guid(u.User.UserId),
                            ProfileImage = u.User.UserProfile.ProfileImage,
                            IsUser = (userId.ToByteArray() == u.User.UserId) ? true : false
                        }).Take(3).ToList()

                    }).ToListAsync<NftSearchData>();

                //foreach (var item in data)
                //{
                //    var dd = await _context.UserNftData.Where(_ => _.Type == item.Chain &&
                //        _.ContractAddress == item.ContractAddress && _.TokenAddress == item.TokenAddress &&
                //        _.TokenId == item.TokenId)
                //        .Select(u => new NftUser
                //        {
                //            Username = u.User.Username,
                //            UserId = new Guid(u.User.UserId),
                //            ProfileImage = u.User.UserProfile.ProfileImage,
                //            IsUser = (userId.ToByteArray() == u.User.UserId) ? true : false
                //        }).Take(3).ToListAsync<NftUser>();

                //    item.Users = dd;
                //}

                response.Data = data;
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

        public async Task<ResponseData> FindNftCollectionAsync(string query, string? cursor, Guid userId)
        {
            try
            {
                var response = new ResponseData();
                query = $"{query.Trim()}%";

                //var exchange = Constant._chainsToValue;

                response.Data = await _context.NftCollectionsData
                    .Include(x => x.UserNftCollectionData)
                    .ThenInclude(x => x.User)
                    .Where(x => EF.Functions.Like(x.ContractName, query) || EF.Functions.Like(x.ContractAddress, query))
                    .AsNoTracking()
                    .AsSplitQuery()
                    .OrderBy(_ => _.ContractName)
                    .ThenBy(_ => _.ContractAddress)
                    .Take(perPage)
                    .Select(g => new
                    {
                        CollectionId = g.Id,
                        CollectionName = g.ContractName,
                        CollectionDescription = g.Description,
                        g.FloorPrice,
                        Chain = g.Chain == Constant.Archway ? "USDC" : g.Chain == Constant.Base ? "ETH" : g.Chain,
                        g.ItemTotal,
                        ImageUrl = g.LogoUrl,
                        UserCount = g.UserNftCollectionData.Count(),

                        //NftImage = g.UserNftCollectionData.OrderBy(_ => _.Id).Take(6).Select(n => new
                        //{
                        //    n.ImageUrl
                        //}).ToList(),

                        Users = g.UserNftCollectionData.Distinct().Select(uc => new
                        {
                            UserId = new Guid(uc.User.UserId),
                            uc.User.Username,
                            uc.User.UserProfile.ProfileImage
                        })
                        .Take(3) 
                    }).ToListAsync();

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

        public async Task<ResponseData> FindUserAsync(string query, int page, Guid userId)
        {
            try
            {
                //var exchange = Constant._chainsToValue;
                //var exchange2 = Constant._chainsToValueFloor;

                var response = new ResponseData();
                query = $"{query.Trim()}%";

                response.Data = await _context.Users
                    .Where(_ => _.Active == 1 && (EF.Functions.Like(_.Username, query) || EF.Functions.Like(_.UserProfile.DisplayName, query)))
                    .Include(_ => _.UserProfile)
                    .Include(_ => _.UserFavoriteNft)
                    .AsNoTracking()
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
                        UserShowcase = _context.UserNftData.Where(_ => _.Favorite == 1 && _.UserId == x.UserId).Select(n => new
                        {
                            n.Id, n.Name, n.ContractAddress, n.TokenId,
                            CollectionId = n.Collection.ParentCollection,
                            n.Chain, n.Description, n.ImageUrl, NativePrice = n.Chain == Constant.Solana ? n.LastTradePrice : n.FloorPrice,
                            USD = CalculatePrice(new Trade(n.LastTradeSymbol, n.LastTradePrice, n.Collection.FloorPrice, n.Chain))

                        }).ToList(),
                        IsFollowing = x.UserFollowerUsers.Where(_ => _.FollowerId == userId.ToByteArray()).FirstOrDefault() != null ? true : false,
                        isVerified = x.VerifiedUsers.Select(x => x.Type).ToList(),
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
    }
}
