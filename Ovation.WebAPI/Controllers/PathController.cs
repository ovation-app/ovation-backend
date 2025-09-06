using Microsoft.AspNetCore.Mvc;
using Ovation.Application.DTOs;
using Ovation.Application.Features.PathFeatures.Requests.Commands;
using Ovation.Application.Features.PathFeatures.Requests.Queries;
using Ovation.WebAPI.Filters;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ovation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PathController : BaseController
    {
        public PathController(IServiceProvider service) : base(service) { }

        // GET: api/<PathController>
        [TokenFilter]
        [HttpGet]
        public async Task<IActionResult> GetPaths()
        {
            var res = await _mediator.Send(new GetPathsQueryRequest());

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        // GET api/<PathController>/5
        [TokenFilter]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPath(Guid id)
        {
            var res = await _mediator.Send(new GetPathQueryRequest(id));

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { Message = "Success", res.Data });
        }

        // POST api/<PathController>
        [CoreFilter]
        [HttpPost]
        public async Task<IActionResult> PostPath([FromBody] PathDto pathDto)
        {
            var res = await _mediator.Send(new AddPathCommandRequest(pathDto));

            if (!res.Status)
                BadRequest(new { res.Message });

            return Ok(new { res.Message, Data = res.GuidValue });
        }

        // PUT api/<PathController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<PathController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
