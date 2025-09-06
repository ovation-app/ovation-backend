using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ovation.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscoverController : BaseController
    {
        public DiscoverController(IServiceProvider service) : base(service) { }

        // GET: api/<DiscoverController>
        [HttpGet("top-nft")]
        public async Task<IActionResult> GetTopNftHolders([FromQuery] int page = 1, [FromQuery] string userPath = "All")
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetTopNftHolderQueryRequest(page, userPath, _userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data, res.RankData });
        }

        [HttpGet("highest-valued-nft")]
        public async Task<IActionResult> GetValuedNftHolders([FromQuery] int page = 1, [FromQuery] string userPath = "All")
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetHighestValuedNftQueryRequest(page, userPath, _userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data, res.RankData });
        }

        [HttpGet("nft-ranking")]
        public async Task<IActionResult> GetHighestValueNft([FromQuery] int page = 1, [FromQuery] string userPath = "All")
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new NftRankingQueryRequest(page, userPath));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        // GET api/<DiscoverController>/5
        [HttpGet("bluechip")]
        public async Task<IActionResult> GetBlueChipHolder([FromQuery] int page = 1, [FromQuery] string userPath = "All")
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetBluechipHoldersQueryRequest(page, userPath, _userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data, res.RankData });
        }

        [HttpGet("networth")]
        public async Task<IActionResult> GetHighestNetworth([FromQuery] int page = 1, [FromQuery] string userPath = "All")
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetNetworthQueryRequest(page, userPath, _userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data, res.RankData });
        }

        [HttpGet("contributors")]
        public async Task<IActionResult> GetContributors([FromQuery] int page = 1)
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetTopContributorsQueryRequest(page));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [HttpGet("creators")]
        public async Task<IActionResult> GetCreators([FromQuery] int page = 1, [FromQuery] string userPath = "All")
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetTopCreatorsQueryRequest(page, userPath));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [HttpGet("founder-nft")]
        public async Task<IActionResult> GetFounderNft([FromQuery] int page = 1, [FromQuery] string userPath = "All")
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetFounderNftRankingQueryRequest(page, userPath, _userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data, res.RankData });
        }

        [HttpGet("most-viewed")]
        public async Task<IActionResult> GetMostViewed([FromQuery] int page = 1)
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetMostViewedQueryRequest(page, _userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

        [HttpGet("most-followed")]
        public async Task<IActionResult> GetMostFollowed([FromQuery] int page = 1)
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetMostFollowedQueryRequest(page, _userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data, res.RankData });
        }

        [HttpGet("most-sold")]
        public async Task<IActionResult> GetMostSold([FromQuery] int page = 1, [FromQuery] string userPath = "All")
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetMostSoldQueryRequest(page, userPath, _userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data, res.RankData });
        }

        [HttpGet("most-sales")]
        public async Task<IActionResult> GetMostSales([FromQuery] int page = 1, [FromQuery] string userPath = "All")
        {
            if (page < 1)
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Invalid page number" });

            var res = await _mediator.Send(new GetMostSalesQueryRequest(page, userPath, _userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data, res.RankData });
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedProfile()
        {
            var res = await _mediator.Send(new GetFeaturedProfileQueryRequest(_userPayload.UserId));

            if (!res.Status) return BadRequest(new { res.Message });

            return Ok(new { res.Message, res.Data });
        }

    }
}
