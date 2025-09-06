using Microsoft.AspNetCore.Mvc;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AssetFeatures.Requests.Queries;
using Ovation.WebAPI.Filters;

namespace Ovation.WebAPI.Controllers
{
    [TokenFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionController(IServiceProvider service, ILogger<object>? logger = null) : BaseController(service, logger)
    {
        [HttpGet("{collectionId}")]
        public async Task<IActionResult> GetCollection(int collectionId)
        {
            var response = await _mediator.Send(new GetCollectionQueryRequest(collectionId));

            if (!response.Status) return BadRequest(new { response.Message });

            return Ok(new { response.Message, response.Data });
        }

        [HttpGet("{collectionId}/tokens")]
        public async Task<IActionResult> GetCollectionTokens(int collectionId, [FromQuery] TokenQueryParametersDto parameters)
        {
            var response = await _mediator.Send(new GetCollectionTokensQueryRequest(collectionId, parameters));

            if (!response.Status) return BadRequest(new { response.Message });

            return Ok(new { response.Message, response.Data, response.Cursor });
        }

        [HttpGet("{collectionId}/owner-distribution")]
        public async Task<IActionResult> GetOwnerDistribution(int collectionId)
        {
            var response = await _mediator.Send(new GetOwnerDistributionQueryRequest(collectionId));

            if (!response.Status) return BadRequest(new { response.Message });

            return Ok(new { response.Message, response.Data });
        }

        [HttpGet("token/{tokenId}")]
        public async Task<IActionResult> GetToken(int tokenId)
        {
            //var response = new ResponseData();

            //if(!isFromProfile)
            var response = await _mediator.Send(new GetTokenQueryRequest(tokenId));
            //else
            //    response = await _mediator.Send(new GetProfileTokenQueryRequest(tokenId));

            if (!response.Status) return BadRequest(new { response.Message });

            return Ok(new { response.Message, response.Data });
        }

        [HttpGet("token/{tokenId}/transactions")]
        public async Task<IActionResult> GetTokenTransactions(int tokenId, [FromQuery] bool isFromProfile = false)
        {
            var response = new ResponseData();

            if (!isFromProfile)
                response = await _mediator.Send(new GetTokenTransactionQueryRequest(tokenId));
            else
                response = await _mediator.Send(new GetProfileTokenTransactionQueryRequest(tokenId));

            if (!response.Status) return BadRequest(new { response.Message });

            return Ok(new { response.Message, response.Data });
        }
    }
}
