using Microsoft.AspNetCore.Mvc;
using Ovation.Application.Features.BadgeFeatures.Requests.Queries;
using Ovation.WebAPI.Filters;

namespace Ovation.WebAPI.Controllers
{
    //[Authorize]
    [TokenFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class BadgeController : BaseController
    {
        public BadgeController(IServiceProvider service) : base(service) { }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBadges([FromRoute] Guid userId, [FromQuery] int page = 1)
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetBadgesQueryRequest(page, userId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [HttpGet("bluechip")]
        public async Task<IActionResult> GetChip([FromQuery] int page = 1)
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetBlueChipsQueryRequest(page));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

    }
}
