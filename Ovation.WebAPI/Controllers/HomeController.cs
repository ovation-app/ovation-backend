using Microsoft.AspNetCore.Mvc;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp;
using Ovation.Application.Features.HomeFeatures.Requests.Queries;
using Ovation.WebAPI.Filters;

namespace Ovation.WebAPI.Controllers
{
    [TokenFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController(IServiceProvider service, ILogger<object>? logger = null) : BaseController(service, logger)
    {
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var res = await _mediator.Send(new GetUsersQueryRequest());

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [HttpGet("nfts")]
        public async Task<IActionResult> GetNfts()
        {
            var res = await _mediator.Send(new GetNftsQueryRequest());

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [HttpPost("wallet")]
        public async Task<IActionResult> GetNftsFromWallet([FromBody] WalletAcct walletDto)
        {
            var res = await _mediator.Send(new GetNftsFromWalletQueryRequest(walletDto));

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { Message = "Success", Data = new { res.Data, profileCompletion = res.IntValue } });
        }
    }
}
