using Microsoft.AspNetCore.Mvc;
using Dynamics365API.Services;
using Dynamics365API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Dynamics365API.Models;

namespace Dynamics365API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICrmService _crmService;

        public AuthController(IAuthService authService, ICrmService crmService)
        {
            _authService = authService;
            _crmService = crmService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //check Email exist in CRM
            var resultCheckEmail = await _crmService.CheckEmailAsync(model.Email);
            if (!resultCheckEmail.IsExisted)
                return BadRequest(resultCheckEmail.Message);

            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("checkEmail")]
        public async Task<IActionResult> CheckEmailAsync([FromBody] EmailForRegistration emailDto)
        {
            var result = await _crmService.CheckEmailAsync(emailDto.Email);

            return Ok(result);
        }

        [AllowAnonymous, HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync(string uid, string token)
        {
            ApplicationUser user = new ApplicationUser() { };
            if (!string.IsNullOrEmpty(uid)) 
            {
                user = await _authService.GetUserByIdAsync(uid);

            }
            EmailConfirmDto model = new EmailConfirmDto
            {
                Email = user?.Email
            };

            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                token = token.Replace(' ', '+');
                var result = await _authService.ConfirmEmailAsync(uid, token);
                if (result.Succeeded)
                {
                    model.EmailVerified = true;
                }
            }

            return Ok(model);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync(EmailConfirmDto model)
        {
            var user = await _authService.GetUserByEmailAsync(model.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    model.EmailVerified = true;
                    return Ok(model);
                }

                await _authService.GenerateEmailConfirmationTokenAsync(user);
                model.EmailSent = true;
                ModelState.Clear();
            }
            else
                return BadRequest("Something went wrong.");

            return Ok(model);
        }

        [AllowAnonymous, HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _authService.GetUserByEmailAsync(model.Email);
                if (user != null)
                    await _authService.GenerateForgotPasswordTokenAsync(user);

                ModelState.Clear();
                model.EmailSent = true;
            }
            return Ok(model);
        }

        [AllowAnonymous, HttpGet("reset-password")]
        public IActionResult ResetPasswordAsync(string uid, string token)
        {
            ResetPasswordDto resetPasswordModel = new ResetPasswordDto
            {
                Token = token,
                UserId = uid
            };
            return Ok(resetPasswordModel);
        }

        [AllowAnonymous, HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                model.Token = model.Token.Replace(' ', '+');
                var result = await _authService.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsSuccess = true;
                    return Ok(model);
                } else
                {
                    var errors = string.Empty;
                    foreach (var error in result.Errors)
                        errors += $"{error.Description},";

                    return Ok(model);
                }
            }
            return Ok(model);
        }
    }
}