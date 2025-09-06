using Microsoft.AspNetCore.Mvc;
using Ovation.Application.Features.WebhookFeatures.Requests.Commands;

namespace Ovation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhooksController(IServiceProvider service, ILogger<object>? logger = null) : BaseController(service, logger)
    {
        [HttpPost("nft-activity")]
        public async Task<IActionResult> PostNftActivity([FromBody] object data)
        {
            await _mediator.Send(new AddNftActivityCommandRequest(data));

            return Ok();
        }
    }
}
