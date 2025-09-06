using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Ovation.Application.Common.Interfaces;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.ApiUser;
using Ovation.Persistence.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ovation.Persistence.Authentication
{
    internal class AuthManager(IConfiguration _configuration, OvationDbContext _context) : IAuthManager
    {
        private UserClaims? _user;
        public string CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims();
            var token = GenerateToken(claims, signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<object?> GetUserDataAsync(byte[] userId)
        {
            try
            {
                var result = await _context.Users
                .Include(u => u.UserProfile)
                //.Include(u => u.UserNfts)
                .Include(u => u.UserPath)
                .Include(u => u.UserSocial)
                .Include(u => u.UserWallets)
                .Include(u => u.UserBadges.Where(b => b.Active == 1))
                .AsNoTracking()
                .AsSplitQuery()
                .Where(u => u.UserId == userId && u.Active == 1)
                .Select(x => new
                {
                    UserId = new Guid(x.UserId),
                    x.Email,
                    x.GoogleId,
                    x.Username,
                    x.UserProfile.DisplayName,
                    x.UserProfile.Bio,
                    x.UserProfile.BirthDate,
                    x.UserProfile.Location,
                    x.UserProfile.ProfileImage,
                    x.UserProfile.CoverImage,
                    Paths = new
                    {
                        Name = x.UserPath != null && x.UserPath.Path != null ? x.UserPath.Path.Name : null,
                        Description = x.UserPath != null && x.UserPath.Path != null ? x.UserPath.Path.Description : null,
                        PathId = x.UserPath != null && x.UserPath.PathId != null ? new Guid(x.UserPath.PathId) : Guid.Empty
                    },

                    Nfts = new List<object>(),

                    Wallets = x.UserWallets.Select(w => new
                    {
                        Id = new Guid(w.Id),
                        w.WalletAddress,
                        w.Wallet.Name,
                        w.Wallet.LogoUrl,
                    }),

                    Socials = x.UserSocial.Socials != null ? JsonConvert.DeserializeObject<List<SocialsDto>>(x.UserSocial.Socials) : null,

                    Badges = x.UserBadges.Select(b => new
                    {
                        BadgeId = new Guid(b.BadgeNameNavigation.BadgeId),
                        b.BadgeNameNavigation.BadgeName,
                        b.BadgeNameNavigation.ImageUrl,
                        b.BadgeNameNavigation.Description
                    })
                })
                .SingleOrDefaultAsync();

                _user = new UserClaims { Email = result.Email, UserId = result.UserId, Username = result.Username };

                return result;
            }
            catch (Exception _)
            {

                return null;
            }

        }

        private JwtSecurityToken GenerateToken(List<Claim> claims, SigningCredentials signingCredentials)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var expiration = DateTime.Now.AddYears(Convert.ToInt16(jwtSettings.GetSection("LifeTime").Value));

            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("Issuer").Value,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
            );
            return token;
        }

        private List<Claim> GetClaims()
        {
            var claims = new List<Claim>();
            if (_user != null)
            {
                claims = new List<Claim>
                {
                    new Claim("Username", _user.Username),
                    new Claim("UserId", _user.UserId.ToString()),
                    new Claim("Email", _user.Email ?? "")
                };
            }

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Constant.OvationKey;
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
    }
}
