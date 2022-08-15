using Dynamics365API.Dtos;
using Dynamics365API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dynamics365API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CrmController : ControllerBase
    {
        private readonly ICrmService _crmService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _authService;

        public CrmController(ICrmService crmService, IHttpContextAccessor httpContextAccessor, IAuthService authService)
        {
            _crmService = crmService;
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;
        }

        [HttpGet("contacts")]
        public async Task<IActionResult> ContactsAsync()
        {
            var result = await _crmService.GetEntityAsync("contacts");
           
            return Ok(result);
        }

        [HttpGet("opportunities")]
        public async Task<IActionResult> OpportunitiesAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var email = user.FindFirst(ClaimTypes.Email).Value;

            var result = await _crmService.GetEntityAsync($"opportunities?$select=name,emailaddress,totalamount,actualclosedate,estimatedclosedate,actualvalue,closeprobability&$filter=emailaddress eq '{email}'");
            
            
            //emailaddress  name    totalamount( Revenu estimé)  actualclosedate    estimatedclosedate   actualvalue(Revenu réel)  closeprobability
            //classement -interes  -tree inter  -peut intrs
            return Ok(result);
        }

        [HttpGet("teamOpportunities")]
        public async Task<IActionResult> TeamOpportunitiesAsync()
        {
            var result = (dynamic)null;

            var user = await _authService.GetCurrentUserAsync(_httpContextAccessor);
            if (user is null)
                return result;

            result = await _crmService.GetTeamOpportunitiesAsync(user.Email);

            return Ok(result);
        }

        [HttpGet("profileDetails")]
        public async Task<IActionResult> ProfileAsync()
        {
            var user = await _authService.GetCurrentUserAsync(_httpContextAccessor);
            var result = await _crmService.GetEntityAsync($"contacts?$select=contactid, firstname,lastname, emailaddress1, jobtitle, telephone1, mobilephone, fax, preferredcontactmethodcode, address1_line1, address1_line2, address1_line3, address1_stateorprovince, address1_postalcode, address1_country, gendercode, familystatuscode, spousesname, birthdate, anniversary&$expand=parentcustomerid_account($select=name)&$filter=emailaddress1 eq '{user.Email}'");

            return Ok(result);
        }

        [HttpGet("allOpportunities")]
        public async Task<IActionResult> AllOpportunitiesAsync()
        {
            var result = await _crmService.GetEntityAsync("opportunities");

            return Ok(result);
        }

        [HttpGet("accounts")]
        public async Task<IActionResult> AccountsAsync()
        {
            var result = await _crmService.GetEntityAsync("accounts");

            return Ok(result);
        }

        [HttpPost("AddAccount")]
        public async Task<IActionResult> AddAccountAsync([FromBody] object obj)
        {
            var result = await _crmService.AddEntityAsync("opportunities?$select=name", obj);
            //?$filter=contains(emailaddress,'miguel@northwindtraders.com')
            return Ok(result);
        }

    }
}
