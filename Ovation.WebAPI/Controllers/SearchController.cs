using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ovation.Application.Features.SearchFeatures.Requests.Queries;

namespace Ovation.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : BaseController
    {
        public SearchController(IServiceProvider service) : base(service) { }

        [HttpGet("user")]
        public async Task<IActionResult> FindUser([FromQuery] string query, [FromQuery] int page = 1)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest(new { Message = "Query can't be empty" });

            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new SearchUserQueryRequest(query, page, _userPayload.UserId));

            return Ok(new { res.Message, res.Data });
        }

        [HttpGet("nft")]
        public async Task<IActionResult> FindNft([FromQuery] string query, [FromQuery] string? next = null)
        {
            if(string.IsNullOrEmpty(query))
                return BadRequest(new { Message = "Query can't be empty" });

            var res = await _mediator.Send(new SearchNftQueryRequest(query, next, _userPayload.UserId));

            return Ok(new { res.Message, res.Data });
        }

        [HttpGet("collection")]
        public async Task<IActionResult> FindCollection([FromQuery] string query, [FromQuery] string? next = null)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest(new { Message = "Query can't be empty" });

            var res = await _mediator.Send(new SearchCollectionQueryRequest(query, next, _userPayload.UserId));

            return Ok(new { res.Message, res.Data });
        }
    }
}
