using Microsoft.AspNetCore.Mvc;
using Ovation.Application.DTOs;
using Ovation.Application.Features.NewsletterFeatures.Requests.Commands;
using Ovation.WebAPI.Filters;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ovation.WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class NewsletterController : BaseController
    {
        public NewsletterController(IServiceProvider service) : base(service) { }

        // POST api/<NewsletterController>
        [TokenFilter]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NewsletterDto value)
        {            
            var result = await _mediator.Send(new AddNewsSubcriberCommandRequest(value));

            if (result.Status)
                return Ok(new { Message = "Subscribed!! Subscriber email saved" });

            if (!result.Status && result.StatusCode == 409)
                return Conflict(new { result.Message });

            return BadRequest(new { result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetArchwayNfts([FromQuery] int page = 1)
        {
            //await _tezosService.GetUserNftsAsync("tz1NUYBXkK58q9yjahxcehAMG7ipabrZLFiF", new Guid("62297F92-1ADD-4CAF-AC96-EF12AFBC94C3"));
            await _mediator.Send(new NuclearPlayGroundCommandRequst(page));
            return Ok();
        }

        //[TokenFilter]
        //[HttpPost("Update")]
        //public async Task<IActionResult> MigrateNft()
        //{
        //    await _newsLetterService.BulkUpdate();
        //    await _newsLetterService.BulkUpdate2();

        //    return Ok();
        //}

        //[HttpGet("migrate2")]
        //public async Task<IActionResult> GetSolBal([FromQuery] string address, [FromQuery] string hexId, [FromQuery] string chain, [FromQuery] bool isMulti = false)
        //{
        //    var user = HelperFunctions.ConvertHexToGuid(hexId);

        //    var res = await _newsLetterService.ResetStatsAsync(user);

        //    if (!res.Status)
        //    {
        //        return BadRequest(new { res.Message });
        //    }

        //    switch (chain)
        //    {
        //        case "solana":
        //            await _solanaService.GetUserNftsAsync(address, user);
        //            break;

        //        case "archway":
        //            await _archwayService.GetUserNftsAsync(address, user);
        //            break;

        //        case "tezos":
        //            await _tezosService.GetUserNftsAsync(address, user);
        //            break;

        //        case "ton":
        //            await _tonService.GetUserNftsAsync(address, user);
        //            break;

        //        default:
        //            if (isMulti)
        //            {
        //                foreach (var item in Constant._evmChains)
        //                    await _evmService.GetUserNftsAsync(address, item, user);
        //            }                    
        //            else
        //                await _evmService.GetUserNftsAsync(address, chain, user);
        //            //if (walletDto.WalletTypeId == null)
        //            //    await _evmService.GetUserNftsAsync(walletDto.WalletAddress, chain, _userPayload.UserId);
        //            //else
        //            //    await _moralisService.GetUserMultiChainNftsAsync(walletDto.WalletAddress, chain, _userPayload.UserId);
        //            break;
        //    }

        //    return Ok(new { Message = $"Migrated: {res.Message}" });
        //}

        //[HttpGet("migrate/all")]
        //public async Task<IActionResult> MigraateAll([FromQuery] int page)
        //{
        //    var users = await _newsLetterService.GetAllUsers(page);

        //    var userCount = users.Count();

        //    var walletCount = 0;

        //    var walletsMigrated = 0;

        //    if (users == null || users.Count == 0) return NotFound();

        //    foreach (var user in users)
        //    {
        //        var res = await _newsLetterService.ResetStatsAsync(user);

        //        if (!res.Status) continue;

        //        var wallets = await _newsLetterService.GetUserWallets(user);

        //        if (wallets == null || wallets.Count == 0) continue;

        //        walletCount += wallets.Count();

        //        foreach (var wallet in wallets)
        //        {
        //            if (string.IsNullOrEmpty(wallet.WalletAddress) || string.IsNullOrEmpty(wallet.Chain)) continue;

        //            var chain = wallet.Chain;
        //            var address = wallet.WalletAddress;

        //            var userId = new Guid(user);
        //            var isMulti = (wallet.WalletId != null) ? true : false;

        //            try
        //            {
        //                switch (chain)
        //                {
        //                    case "solana":
        //                        await _solanaService.GetUserNftsAsync(address, userId);
        //                        break;

        //                    case "archway":
        //                        await _archwayService.GetUserNftsAsync(address, userId);
        //                        break;

        //                    case "tezos":
        //                        await _tezosService.GetUserNftsAsync(address, userId);
        //                        break;

        //                    case "ton":
        //                        await _tonService.GetUserNftsAsync(address, userId);
        //                        break;

        //                    default:
        //                        if (isMulti)
        //                        {
        //                            foreach (var item in Constant._evmChains)
        //                                await _evmService.GetUserNftsAsync(address, item, userId);
        //                        }
        //                        else
        //                            await _evmService.GetUserNftsAsync(address, chain, userId);
        //                        //if (walletDto.WalletTypeId == null)
        //                        //    await _evmService.GetUserNftsAsync(walletDto.WalletAddress, chain, _userPayload.UserId);
        //                        //else
        //                        //    await _moralisService.GetUserMultiChainNftsAsync(walletDto.WalletAddress, chain, _userPayload.UserId);
        //                        break;
        //                }

        //                walletsMigrated++;
        //            }
        //            catch (Exception _)
        //            {
        //                continue;
        //            }


        //        }

        //    }

        //    return Ok(new { Message = $"Migrated: {userCount} users with total wallets of {walletCount} and successfull migration of {walletsMigrated}" });
        //}

        //[HttpGet("test3")]
        //public async Task<IActionResult> GetEnv([FromQuery] int page = 1)
        //{
        //    await _profileService.DoUpdateAsync(page);

        //    return Ok();
        //}

        //[TokenFilter]
        //[HttpGet("first")]
        //public async Task<IActionResult> GetWorth([FromQuery] int page)
        //{
        //    if (page < 1)
        //        return BadRequest();

        //    var res = await _newsLetterService.SendFirstLaunchEmail(page);

        //    if (!res.Status)
        //        return BadRequest(new { res.Message });

        //    return Ok(new { res.Message });
        //}

        //[TokenFilter]
        //[HttpGet("second")]
        //public async Task<IActionResult> Get2Worth([FromQuery] int page)
        //{
        //    if (page < 1)
        //        return BadRequest();

        //    var res = await _newsLetterService.SendSecondLaunchEmail(page);

        //    if (!res.Status)
        //        return BadRequest(new { res.Message });

        //    return Ok(new { res.Message });
        //}

        //[TokenFilter]
        //[HttpPost("public")]
        //public async Task<IActionResult> SendPublicLaunchEmail([FromQuery] int page = 1)
        //{
        //    if (page < 1)
        //        return BadRequest();

        //    var res = await _newsLetterService.SendPublicLaunchEmail(page);

        //    if (!res.Status)
        //        return BadRequest(new { res.Message });

        //    return Ok(new { res.Message });
        //}
    }
}
