using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ovation.Application.Features.FeedbackFeatures.Requests.Commands;
using Ovation.Application.DTOs;

namespace Ovation.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : BaseController
    {
        public FeedbackController(IServiceProvider service) : base(service) { }

        [HttpPost]
        public async Task<IActionResult> AddFeedback([FromBody] UserFeedbackDto feedback)
        {
            var res = await _mediator.Send(new AddFeedbackCommandRequest(feedback, _userPayload.UserId));

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }
    }
}
