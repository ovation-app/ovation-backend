using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ovation.Application.Common.Interfaces;
using Ovation.Application.DTOs.ApiUser;

namespace Ovation.WebAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        public readonly ILogger<object> _logger;

        public readonly IMediator _mediator;

        public readonly IServiceProvider service;
        public readonly IUserManager? _userManager;
        public readonly UserPayload _userPayload;
       
        public BaseController(IServiceProvider service, ILogger<object>? logger = default)
        {
            _userManager = service.GetService<IUserManager>();
            _userPayload = _userManager.GetUserPayload();
            
            _mediator = service.GetRequiredService<IMediator>();

            this.service = service;
            if (logger != null)
                _logger = logger;

            if (_userPayload != null)
            {
                SentrySdk.ConfigureScope(scope =>
                {
                    scope.User = new SentryUser
                    {
                        Username = _userPayload.Username,
                        Email = _userPayload.Email,
                        Id = _userPayload.UserId.ToString()
                        
                    };

                    scope.SetTag("ovation.username", _userPayload.Username);
                });


            }
        }
    }
}
