using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp;
using Ovation.Application.Features.AuthFeatures.Requests.Queries;
using Ovation.Application.Features.AuthFeatures.Requests.Commands;
using Ovation.Persistence.Services;
using Ovation.WebAPI.Filters;
using Ovation.Application.DTOs.SignUp.Claimable;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ovation.WebAPI.Controllers
{
    [TokenFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        public AuthController(IServiceProvider service) : base(service) { }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userData = await _mediator.Send(new AuthUserDataQueryRequest(_userPayload.UserId));

            if (userData == null)
                return BadRequest(new { Message = "An error occurred" });

            return Ok(new { Message = "Account data fetched", userData });
        }

        [HttpPost("email")]
        public async Task<IActionResult> CheckEmail([FromBody] EmailDto user)
        {
            if (string.IsNullOrEmpty(user.Email))
                return BadRequest(new { Message = "Invalid Email Address" });

            if (!HelperFunctions.IsValidEmail(user.Email))
                return BadRequest(new { Message = "Invalid Email Address" });

            var res = await _mediator.Send(new CheckEmailQueryRequest(user.Email));

            if (!res)
                return Ok(new { Message = "Email not found" });

            return Conflict(new { Message = "Email found" });
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> CheckUsername([FromRoute] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest(new { Message = "Invalid Email Address" });

            var res = await _mediator.Send(new CheckUsernameQueryRequest(username));

            if (!res)
                return Ok(new { Message = "Username not found" });

            return Conflict(new { Message = "Username found" });
        }

        [HttpPost("user")]
        public async Task<IActionResult> CheckUser([FromBody] SocialLoginDto user)
        {
            if (!user.SocialId.Contains('@'))
                user.SocialId = $"{user.SocialId}@ovation.network";

            var response = await _mediator.Send(new GetUserDataCommandRequest(user.SocialId));

            if (response == null)
                return NotFound(new { Message = "User not found" });

            if(response.UserData == null || string.IsNullOrEmpty(response.Token))
                return BadRequest(new { Message = "An error occurred" });

            return Ok(new { Message = "Success", response.UserData, response.Token });
        }

        // POST api/<AuthController>
        [HttpPost("register")]
        public async Task<IActionResult> Post([FromBody] RegisterDto registerDto)
        {
            var response = await _mediator.Send(new RegisterUserCommandRequest(registerDto));

            if (response.UserId == Guid.Empty && (response.UserData == null || string.IsNullOrEmpty(response.Token)))
                return BadRequest(new { response.Message });

            if(response.UserId != Guid.Empty && (response.UserData == null || string.IsNullOrEmpty(response.Token)))
                return Accepted( new { response.Message, response.UserId });

            return Ok(new { Message = "Success", response.UserData, response.Token });
        }

        [HttpPost("claimable/register")]
        public async Task<IActionResult> PostClaimable([FromBody] ClaimableProfile registerDto)
        {
            var response = await _mediator.Send(new CreateClaimableProfileCommandRequest(registerDto));

            if(!response.Status)
                return BadRequest(new { response.Message });

            return Ok(new { response.Message });
        }

        [HttpGet("account/verify/{code}/{otp}")]
        public async Task<IActionResult> VerifyAccount(Ulid code, int otp)
        {
            var res = await _mediator.Send(new VerifyAcctQueryRequest(code, otp));

            if (res.UserData == null || string.IsNullOrEmpty(res.Token)) return BadRequest(res.Message);

            return Ok(new { res.Message, res.UserData, res.Token });
        }

        [HttpGet("account/verify/{userId}")]
        public async Task<IActionResult> IsVerifyAccount(Guid userId)
        {
            var isVerified = await _mediator.Send(new CheckVerifyAcctQueryRequest(userId));

            return Ok(new { isVerified });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var res = await _mediator.Send(new NormalLoginCommandRequest(loginDto));

            if (res.UserData == null || string.IsNullOrEmpty(res.Token)) return BadRequest(res.Message);

            return Ok(new { res.Message, res.UserData, res.Token });
        }

        [HttpPost("login/google")]
        public async Task<IActionResult> GoogleLogin([FromBody] SocialLoginDto social)
        {
            var res = await _mediator.Send(new GoogleLoginCommandRequest(social.SocialId));

            if (res.UserData == null || string.IsNullOrEmpty(res.Token)) return BadRequest(res.Message);

            return Ok(new { res.Message, res.UserData, res.Token });
        }

        [HttpPost("login/x")]
        public async Task<IActionResult> XLogin([FromBody] SocialLoginDto social)
        {
            var res = await _mediator.Send(new XLoginCommandRequest(social.SocialId));

            if (res.UserData == null || string.IsNullOrEmpty(res.Token)) return BadRequest(res.Message);

            return Ok(new { res.Message, res.UserData, res.Token });
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] EmailDto emailDto)
        {            
            var res = await _mediator.Send(new ForgetPasswordQueryRequest(emailDto.Email));

            if (res.StatusCode == 404 && !res.Status)
                return NotFound(new { res.Message });

            if (!res.Status)
                return BadRequest(new { res.Message });

            return Ok(new { Message = "OTP Sent!", Data = res.GuidValue });
        }

        [HttpGet("verify/otp/{userId}/{otp}")]
        public async Task<IActionResult> VerifyOTP([FromRoute] Guid userId, int otp)
        {
            var res = await _mediator.Send(new VerifyOtpQueryRequest(userId, otp));

            if (res.Status)
                return Ok(new { res.Message });

            return BadRequest(new { res.Message });
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var res = await _mediator.Send(new ChangePasswordCommandRequest(changePasswordDto));

            if (!res.Status)
                return BadRequest(new { Message = $"{res.Message}" });

            return Ok(new { Message = "Password changed successfully" });
        }

    }
}
