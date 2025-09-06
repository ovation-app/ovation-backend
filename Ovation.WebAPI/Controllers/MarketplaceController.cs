using Microsoft.AspNetCore.Mvc;
using Ovation.Application.Features.MarketplaceFeatures.Requests.Queries;
using Ovation.WebAPI.Filters;

namespace Ovation.WebAPI.Controllers
{
    [TokenFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class MarketplaceController(IServiceProvider service) : BaseController(service)
    {
        [HttpGet]
        public async Task<IActionResult> GetMarketplaceData(
            [FromQuery] string? cursor = null, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? sortDirection = null)
        {

            var res = await _mediator.Send(new GetMarketplaceQueryRequest(cursor, pageSize, sortDirection));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }
    }
}
