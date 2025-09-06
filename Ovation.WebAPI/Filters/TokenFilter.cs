using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ovation.Application.Features.DevTokenFeatures.Requests.Queries;

namespace Ovation.WebAPI.Filters
{
    public class TokenFilter : Attribute, IAuthorizationFilter
    {
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                if (context.HttpContext.Request.Headers.ContainsKey("tokenId"))
                {
                    var token = Guid.Parse(context.HttpContext.Request.Headers["tokenId"]!);

                    var mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();

                    if (!await mediator.Send(new TokenFilterQueryRequest(token)))
                    {
                        context.Result = new UnauthorizedResult();
                        return;
                    }
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

            }
            catch (Exception)
            {
                context.Result = new BadRequestObjectResult(new { Status = 400, Message = "An Error Occurred" });
                return;
            }
        }
    }
}
