using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp;
using Ovation.Application.Features.AssetFeatures.Requests.Queries;
using Ovation.Application.Features.FollowFeatures.Requests.Commands;
using Ovation.Application.Features.FollowFeatures.Requests.Queries;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.DELETE;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.POST;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;
using Ovation.WebAPI.Filters;

namespace Ovation.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : BaseController
    {
        public ProfileController(IServiceProvider service) : base(service) { }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var res = await _mediator.Send(new GetAuthUserQueryRequest(_userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(username))
                return BadRequest(new { Message = "Invalid username" });

            var user = _userPayload != null ? _userPayload.UserId : Guid.Empty;

            var res = await _mediator.Send(new GetUserQueryRequest(username, user));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("experience/{userId}")]
        public async Task<IActionResult> GetExperiences(Guid userId, [FromQuery] int page = 1)
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var res = await _mediator.Send(new GetUserExperiencesQueryRequest(userId, page));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("nft/{userId}")]
        public async Task<IActionResult> GetNfts(Guid userId, [FromQuery] NftQueryParametersDto parameters)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var user = _userPayload != null ? _userPayload.UserId : Guid.Empty;

            var res = await _mediator.Send(new GetUserNftsQueryRequest(userId, user, parameters));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data, res.Cursor });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("nft/top/{userId}")]
        public async Task<IActionResult> GetTopNfts(Guid userId, [FromQuery] int limit)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var res = await _mediator.Send(new GetUserTopNftsQueryRequest(limit, userId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("nft/{userId}/{assetId}")]
        public async Task<IActionResult> GetNfts(Guid userId, [FromRoute] int assetId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            //var user = _userPayload != null ? _userPayload.UserId : Guid.Empty;

            var res = await _mediator.Send(new GetProfileTokenQueryRequest(assetId, userId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data, res.Cursor });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("collection/{userId}")]
        public async Task<IActionResult> GetCollections(Guid userId, [FromQuery] NftQueryParametersDto parameters)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var user = _userPayload != null ? _userPayload.UserId : Guid.Empty;

            var res = await _mediator.Send(new GetUserCollectionQueryRequest(userId, user, parameters));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data, res.Cursor });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("nft/overview/{userId}")]
        public async Task<IActionResult> GetNftOverview(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var res = await _mediator.Send(new GetNftOverviewQueryRequest(userId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("fav-nft/{userId}")]
        public async Task<IActionResult> GetFavNfts(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var res = await _mediator.Send(new GetUserFavNftQueryRequest(userId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            if (res.Data == null)
                return NotFound(new { Message = "No Favorite Nft Found" });

            return Ok(new { res.Message, res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("portfolio-record/{userId}")]
        public async Task<IActionResult> GetPortfolioRecord(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var res = await _mediator.Send(new GetUserPortfolioQueryRequest(userId));

            if (!res.Status)
                return NotFound(new { Message = "No portfolio recordd" });

            return Ok(new { Message = "Success", res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("transaction/{userId}")]
        public async Task<IActionResult> GetNftTransactions(Guid userId, [FromQuery] string? next = null)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var res = await _mediator.Send(new GetUserTransactionsQueryRequest(userId, next));

            if (res == null)
                return NotFound(new { Message = "No transaction data Found" });

            return Ok(new { Message = "Success", res.Data, res.Cursor });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("featured/{userId}")]
        public async Task<IActionResult> GetFeaturedNfts(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var res = await _mediator.Send(new GetUserFeaturedItemQueryRequest(userId));

            if (res == null || res.Count == 0)
                return NotFound(new { Message = "No Featured Nft Found" });

            return Ok(new { Message = "Success", Data = res });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("badge/{userId}")]
        public async Task<IActionResult> GetBadge(Guid userId, [FromQuery] int page)
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var res = await _mediator.Send(new GetUserBadgesQueryRequest(userId, page));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("social/{userId}")]
        public async Task<IActionResult> GetSocial(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var res = await _mediator.Send(new GetUserSocialsQueryRequest(userId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("stat/{userId}")]
        public async Task<IActionResult> GetStat(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            var res = await _mediator.Send(new GetUserStatQueryRequest(userId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("followers/{userId}")]
        public async Task<IActionResult> GetFollower(Guid userId, [FromQuery] int page = 1)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            if (page < 1)
                return BadRequest(new { Message = "Page number can't be less than one" });

            var user = _userPayload != null ? _userPayload.UserId : Guid.Empty;

            var res = await _mediator.Send(new GetUserFollowersQueryRequest(userId, page, user));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("followings/{userId}")]
        public async Task<IActionResult> GetFollowing(Guid userId, [FromQuery] int page = 1)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "Invalid userId" });

            if (page < 1)
                return BadRequest(new { Message = "Page number can't be less than one" });

            var user = _userPayload != null ? _userPayload.UserId : Guid.Empty;

            var res = await _mediator.Send(new GetUserFollowingsQueryRequest(userId, page, user));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("followers/user/{username}")]
        public async Task<IActionResult> GetFollower(string username, [FromQuery] int page = 1)
        {
            if (page < 1)
                return BadRequest(new { Message = "Page number can't be less than one" });

            var user = _userPayload != null ? _userPayload.UserId : Guid.Empty;

            var res = await _mediator.Send(new GetUsernameFollowersQueryRequest(username, page, user));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("followings/user/{username}")]
        public async Task<IActionResult> GetFollowing(string username, [FromQuery] int page = 1)
        {
            if (page < 1)
                return BadRequest(new { Message = "Page number can't be less than one" });
            
            var user = _userPayload != null ? _userPayload.UserId : Guid.Empty;

            var res = await _mediator.Send(new GetUsernameFollowingsQueryRequest(username, page, user));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [AllowAnonymous]
        [TokenFilter]
        [HttpGet("wallet/{userId}")]
        public async Task<IActionResult> GetWallets(Guid userId)
        {
            var res = await _mediator.Send(new GetWalletsQueryRequest(userId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [TokenFilter]
        [HttpGet("wallet")]
        public async Task<IActionResult> GetUserWallets()
        {
            var res = await _mediator.Send(new GetUserWalletsQueryRequest(_userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [TokenFilter]
        [HttpGet("verification")]
        public async Task<IActionResult> GetUserVerification()
        {
            var res = await _mediator.Send(new GetUserVerificationQueryRequest(_userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            if (res.Data == null)
                return NotFound(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [HttpPost("experience")]
        public async Task<IActionResult> AddExperience([FromBody] UserExperienceDto userExperience)
        {
            var res = await _mediator.Send(new AddExperienceCommandRequest(userExperience, _userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.GuidValue });
        }

        [HttpPost("wallet")]
        public async Task<IActionResult> AddWallet([FromBody] WalletAcct walletDto)
        {
            try
            {                
                var res = await _mediator.Send(new AddUserWalletCommandRequest(walletDto, _userPayload.UserId));

                if (res.StatusCode == 409)
                    return Conflict(new { Message = "Wallet address already exist" });

                if (!res.Status)
                    return BadRequest(new { res.Message });

                return Ok(new { res.Message });
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return BadRequest(new { new ResponseData().Message });
            }
        }

        [HttpPost("follow/{userId}")]
        public async Task<IActionResult> FollowUser([FromRoute] Guid userId)
        {
            if (userId == _userPayload.UserId)
                return BadRequest(new { Message = "Invalid Actions" });

            var res = await _mediator.Send(new FollowUserCommandRequest(userId, _userPayload.UserId));

            if (!res.Status)
                return Accepted(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPost("view/{userId}")]
        public async Task<IActionResult> ViewUserProfile([FromRoute] Guid userId)
        {
            var res = await _mediator.Send(new ViewProfileCommandRequest(userId, _userPayload.UserId));

            if (!res.Status)
                return Accepted(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPost("verification")]
        public async Task<IActionResult> VerifyUser([FromBody] VerifiyUserDTO verifiyUser)
        {
            if (!Enum.IsDefined(typeof(VerificationTypes), verifiyUser.VerificationType))
                return BadRequest(new { Message = "Invalid verification type" });

            var res = await _mediator.Send(new VerifyUserCommandRequest(verifiyUser, _userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPatch("personal-info")]
        public async Task<IActionResult> UpdateInfo([FromBody] ProfileModDto profileDto)
        {            
            var res = await _mediator.Send(new UpdateProfileCommandRequest(profileDto, _userPayload.UserId));

            if (res.StatusCode == 409)
                return Conflict(new { res.Message });

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPatch("user-path")]
        public async Task<IActionResult> UpdateUserPath([FromBody] UserPathDto userPath)
        {
            var res = await _mediator.Send(new UpdateUserPathCommandRequest(userPath, _userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPatch("cover-image")]
        public async Task<IActionResult> UpdateCoverImage([FromBody] ImageDto imageDto)
        {
            var res = await _mediator.Send(new UpdateCoverImageCommandRequest(imageDto.ImageUrl, _userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPatch("experience/{id}")]
        public async Task<IActionResult> UpdateExperience([FromRoute] Guid id, [FromBody] UserExperienceModDto expDto)
        {
            var res = await _mediator.Send(new UpdateExperienceCommandRequest(expDto, id, _userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPatch("socials")]
        public async Task<IActionResult> UpdateSocials([FromBody] UserSocialsModDto socialDto)
        {
            var res = await _mediator.Send(new UpdateSocialsCommandRequest(socialDto, _userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPatch("fav-nft")]
        public async Task<IActionResult> UpdateFavNft([FromBody] List<IntId> favDto)
        {
            var res = await _mediator.Send(new UpdateFavNftCommandRequest(favDto, _userPayload.UserId));

            if (res.StatusCode == 404)
                return NotFound(new { res.Message });

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPatch("featured-item")]
        public async Task<IActionResult> UpdateFeaturedItem([FromBody] IdsWithTypeDto favDto)
        {
            if (favDto.FeatureItems.Count > 4)
                return BadRequest(new { Message = "Only 4 items can be stored" });

            var res = await _mediator.Send(new UpdateFeatureItemCommandRequest(favDto, _userPayload.UserId));

            if (res.StatusCode == 404)
                return NotFound(new { res.Message });

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPatch("nft/privacy")]
        public async Task<IActionResult> UpdateNftPrivacy([FromBody] List<NftVisibleDto> nftDto)
        {
            var res = await _mediator.Send(new UpdateNftPrivacyCommandRequest(nftDto, _userPayload.UserId));

            if (res.StatusCode == 404)
                return NotFound(new { res.Message });

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordModDto passwordDto)
        {
            var res = await _mediator.Send(new ChangePasswordCommandRequest(passwordDto, _userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Password changed successfully" });
        }

        [HttpPatch("nft/{nftId}/sale")]
        public async Task<IActionResult> UpdateNftSale(long nftId, [FromBody] UpdateNftSaleDto request)
        {
            var res = await _mediator.Send(new UpdateNftSaleCommandRequest( _userPayload.UserId, nftId, request));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [HttpDelete("follow/{userId}")]
        public async Task<IActionResult> UnfollowUser([FromRoute] Guid userId)
        {
            var res = await _mediator.Send(new UnfollowUserCommandRequest(userId, _userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpDelete("fav-nft")]
        public async Task<IActionResult> DeleteFavNft([FromBody] DeleteFavNft deleteFavNft)
        {
            var res = await _mediator.Send(new RemoveNftFromFavCommandRequest(deleteFavNft.Data, _userPayload.UserId));

            if (res.StatusCode == 404)
                return NotFound(new { res.Message });

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpDelete("wallet/{id}")]
        public async Task<IActionResult> DeleteWallet([FromRoute] Guid id)
        {
            var res = await _mediator.Send(new DeleteWalletCommandRequest(id, _userPayload.UserId));

            if (res.StatusCode == 404)
                return NotFound(new { res.Message });

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpDelete("wallet/group/{groupId}")]
        public async Task<IActionResult> DeleteWalletGroup([FromRoute] Guid groupId)
        {
            var res = await _mediator.Send(new DeleteWalletGroupCommandRequest(groupId, _userPayload.UserId));

            if (res.StatusCode == 404)
                return NotFound(new { res.Message });

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpDelete("experience/{id}")]
        public async Task<IActionResult> DeleteExperience([FromRoute] Guid id)
        {
            var res = await _mediator.Send(new DeleteExperienceCommandRequest(id, _userPayload.UserId));

            if (res.StatusCode == 404)
                return NotFound(new { res.Message });

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpDelete("verification/{verifyType}")]
        public async Task<IActionResult> UnverifyUser([FromRoute] string verifyType)
        {
            if (!Enum.IsDefined(typeof(VerificationTypes), verifyType))
                return BadRequest(new { Message = "Invalid verification type" });

            var res = await _mediator.Send(new UnverifyUserCommandRequest(verifyType, _userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }
    }
}
