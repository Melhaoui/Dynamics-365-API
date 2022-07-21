using Dynamics365API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dynamics365API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrmController : ControllerBase
    {
        private readonly ICrmService _crmService;

        public CrmController(ICrmService crmService)
        {
            _crmService = crmService;
        }

        [HttpGet("contacts")]
        public async Task<IActionResult> ContactsAsync()
        {
            var result = await _crmService.GetEntity("contacts");

            return Ok(result);
        }
    }
}
