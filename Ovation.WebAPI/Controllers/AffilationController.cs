using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ovation.Application.Features.AffilationFeatures.Requests.Queries;

namespace Ovation.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AffilationController : BaseController
    {
        public AffilationController(IServiceProvider service) : base(service) { }

        [HttpGet]
        public async Task<IActionResult> GetUserAffilation()
        {
            var res = await _mediator.Send(new GetAffilationDataQueryRequest(_userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [HttpGet("invited")]
        public async Task<IActionResult> GetUserInvited()
        {
            var res = await _mediator.Send(new GetInvitedUserQueryRequest(_userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }
    }
}
