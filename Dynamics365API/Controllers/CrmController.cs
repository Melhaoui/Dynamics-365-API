using Dynamics365API.Dtos;
using Dynamics365API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace Dynamics365API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
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
            string Query = $"opportunities" +
                           $"?$select=name,emailaddress,totalamount,actualclosedate,estimatedclosedate,actualvalue,closeprobability" +
                           $"&$filter=emailaddress eq '{email}'";
            var result = await _crmService.GetEntityAsync(Query);
            
            
            //emailaddress  name    totalamount( Revenu estimé)  actualclosedate    estimatedclosedate   actualvalue(Revenu réel)  closeprobability
            //classement -interes  -tree inter  -peut intrs
            return Ok(result);
        }

        [HttpGet("teamOpportunities")]
        public async Task<IActionResult> TeamOpportunitiesAsync()
        {
            var result = (dynamic)null;
            var user = await _authService.GetCurrentUserAsync(_httpContextAccessor);
            var allEmailTeamNotPrimaryFilter = await _crmService.GetAllEmailTeamAsync(user.Email, "emailaddress");
            string Query = $"opportunities" +
                           $"?$select=name,emailaddress,totalamount" +
                           $",actualclosedate,estimatedclosedate,actualvalue,closeprobability" +
                           $"&$filter={allEmailTeamNotPrimaryFilter} ";
            result = await _crmService.GetEntityAsync(Query);

            return Ok(result);
        }

        [HttpGet("teamContacts")]
        public async Task<IActionResult> TeamContactsAsync()
        {
            var result = (dynamic)null;
            var user = await _authService.GetCurrentUserAsync(_httpContextAccessor);
            var allEmailTeamNotPrimaryFilter = await _crmService.GetAllEmailTeamAsync(user.Email, "emailaddress1");
            string Query = $"contacts" +
                           $"?$select=fullname, emailaddress1, telephone1" +
                           $"&$filter={allEmailTeamNotPrimaryFilter} ";
            result = await _crmService.GetEntityAsync(Query);

            return Ok(result);
        }

        [HttpGet("profileDetails")]
        public async Task<IActionResult> ProfileAsync()
        {
            var user = await _authService.GetCurrentUserAsync(_httpContextAccessor);
            string Query =
                $"contacts" +
                $"?$select=contactid, firstname,lastname, jobtitle, emailaddress1, telephone1, mobilephone, fax, " +
                $"preferredcontactmethodcode, address1_line1, address1_line2, address1_line3, address1_city, address1_stateorprovince," +
                $"address1_postalcode, address1_country, entityimage, gendercode, familystatuscode, spousesname, birthdate, anniversary" +
                $"&$expand=parentcustomerid_account($select=name)" +
                $"&$filter=emailaddress1 eq '{user.Email}'";
            var result = await _crmService.GetEntityAsync(Query);

            return Ok(result);
        }

        [HttpPut("profileUpdate")]
        public async Task<IActionResult> ProfileUpdateAsync([FromBody] ProfileDto profileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _authService.GetCurrentUserAsync(_httpContextAccessor);
            string Query = $"contacts?$select=contactid&$filter=contains(emailaddress1, '{user.Email}')";
            var contact = await _crmService.GetEntityAsync(Query);
            var values = JObject.Parse(contact.ToString());
            var Contactid = values.SelectToken("value[0].contactid").ToString();
            Contactid = Contactid.Trim(new Char[] { '{', '}' });
            var result = await _crmService.CrmCrud(HttpMethod.Patch, $"contacts({Contactid})", profileDto);

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


    }
}
