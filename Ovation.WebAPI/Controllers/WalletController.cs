using Microsoft.AspNetCore.Mvc;
using Ovation.Application.DTOs;
using Ovation.WebAPI.Filters;
using Ovation.Application.Features.WalletFeatures.Requests.Queries;
using Ovation.Application.Features.WalletFeatures.Requests.Commands;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ovation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : BaseController
    {
        public WalletController(IServiceProvider service) : base(service) { }

        // GET: api/<WalletController>
        [TokenFilter]
        [HttpGet]
        public async Task<IActionResult> GetWallets()
        {
            var res = await _mediator.Send(new GetWalletsQueryRequest());

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        // GET api/<PathController>/5
        [TokenFilter]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWallet(Guid id)
        {
            var res = await _mediator.Send(new GetWalletQueryRequest(id));

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        // POST api/<PathController>
        [CoreFilter]
        [HttpPost]
        public async Task<IActionResult> PostWallet([FromBody] WalletDto walletDto)
        {
            var res = await _mediator.Send(new AddWalletCommandRequest(walletDto));

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { res.Message, Data = res.GuidValue });
        }
    }
}
