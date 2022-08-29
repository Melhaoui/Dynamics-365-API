using Dynamics365API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dynamics365API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ICrmService _crmService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _authService;

        public DashboardController(ICrmService crmService, IHttpContextAccessor httpContextAccessor, IAuthService authService)
        {
            _crmService = crmService;
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;
        }

        [HttpGet("opportunitiesStatusCodeCount")]
        public async Task<IActionResult> OpportunitiesStatusCodeCountAsync()
        {
            var user = await _authService.GetCurrentUserAsync(_httpContextAccessor);
            var result = await _crmService.GetOpportunitiesStatusCodeCountAsync(user.Email);

            return Ok(result);
        }

        [HttpGet("opportunitiesEstimatedRevenue")]
        public async Task<IActionResult> OpportunitiesEstimatedRevenue()
        {
            var user = await _authService.GetCurrentUserAsync(_httpContextAccessor);
            var result = await _crmService.GetOpportunitiesEstmatedRevenueAsync(user.Email);

            return Ok(result);
        }
    }
}
