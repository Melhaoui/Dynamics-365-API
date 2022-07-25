using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dynamics365API.Services;
using Dynamics365API.Dtos;

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
    }
}