using Dynamics365API.Dtos;
using Dynamics365API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Dynamics365API.Services
{
    public class CrmService : ICrmService
    {
        private readonly CRM _crm;

        public CrmService(CRM crm)
        {
            _crm = crm;
        }

        public async Task<CrmCheckEmailDto> CheckEmailAsync(string email)
        {
            var crmCheckEmail = new CrmCheckEmailDto { Message = "My Email is saying it does not exist", Email = email };
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();

            var result = await httpClient.GetFromJsonAsync<CrmCheckEmailDataDto>(organizationAPIUrl + "contacts?$select=fullname,emailaddress1&$filter=contains(emailaddress1,'" + email + "')");
            if (result?.value?.Count == 1)
            {
                crmCheckEmail.Message = "This email address is already exist.";
                crmCheckEmail.IsExisted = true;
            }
            return crmCheckEmail;
        }

        public async Task<object> GetEntity(string entityQuery)
        {
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();
            
            var result = await httpClient.GetFromJsonAsync<object>(organizationAPIUrl + entityQuery);
           
            return  result ;
        }
        //------------------------------AddEntity Not working------------------------------------------------------------------
        public async Task<string> AddEntity(string entityQuery,object jsonObject)
        {
      
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();
           
            var content = new StringContent(
                 JsonConvert.SerializeObject(jsonObject).ToString(), Encoding.UTF8, "application/json");
            var httpResponse =  await httpClient.PostAsync(organizationAPIUrl + entityQuery, content);

            var result = await httpResponse.Content.ReadAsStringAsync();

            return result;
        }

    }
}
