using Dynamics365API.Dtos;
using Dynamics365API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dynamics365API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrmController : ControllerBase
    {
        private readonly ICrmService _crmService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CrmController(ICrmService crmService, IHttpContextAccessor httpContextAccessor)
        {
            _crmService = crmService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("contacts")]
        public async Task<IActionResult> ContactsAsync()
        {
            var result = await _crmService.GetEntity("contacts");
           
            return Ok(result);
        }

        [HttpGet("opportunities")]
        public async Task<IActionResult> OpportunitiesAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var email = user.FindFirst(ClaimTypes.Email).Value;

            var result = await _crmService.GetEntity($"opportunities?$select=name,emailaddress,totalamount,actualclosedate,estimatedclosedate,actualvalue,closeprobability&$filter=emailaddress eq '{email}'");
            
            
            //emailaddress  name    totalamount( Revenu estimé)  actualclosedate    estimatedclosedate   actualvalue(Revenu réel)  closeprobability
            //classement -interes  -tree inter  -peut intrs
            return Ok(result);
        }

        [HttpPost("AddAccount")]
        public async Task<IActionResult> AddAccountAsync([FromBody] object obj)
        {
            var result = await _crmService.AddEntity("opportunities?$select=name", obj);
            //?$filter=contains(emailaddress,'miguel@northwindtraders.com')
            return Ok(result);
        }

    }
}
