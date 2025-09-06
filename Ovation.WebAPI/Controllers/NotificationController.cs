using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ovation.Application.Features.NotificationFeatures.Requests.Queries;
using Ovation.Application.Features.NotificationFeatures.Requests.Commands;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ovation.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        public NotificationController(IServiceProvider service) : base(service) { }


        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int page = 1)
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetNotificationsQueryRequest(_userPayload.UserId, page));

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetUnreadNotificationsCount()
        {
            var res = await _mediator.Send(new GetNotificationCountQueryRequest(_userPayload.UserId));

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        [HttpPatch("view/{id}")]
        public async Task<IActionResult> ReadNotification([FromRoute] Guid id)
        {
            var res = await _mediator.Send(new ReadNotificationCommandRequest(id, _userPayload.UserId));

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }

        [HttpPatch("view")]
        public async Task<IActionResult> ReadAllNotification()
        {
            var res = await _mediator.Send(new ReadAllNotificationsCommandRequest(_userPayload.UserId));

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { res.Message });
        }
    }
}
