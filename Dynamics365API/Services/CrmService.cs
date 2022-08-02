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
        private readonly IAuthService _authService;

        public CrmService(CRM crm, IAuthService authService)
        {
            _crm = crm;
            _authService = authService;
        }

        public async Task<CrmCheckEmailDto> CheckEmailAsync(string email)
        {
            var crmCheckEmail = new CrmCheckEmailDto { Message = "My Email is saying it does not exist", Email = email };

            //check Email exists DB
            if (await _authService.GetUserByEmailAsync(email) is not null)
            {
                crmCheckEmail.Message = "Email is already registered!";
                return crmCheckEmail;
            }
                

            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();

            var result = await httpClient.GetFromJsonAsync<CrmCheckEmailDataDto>(organizationAPIUrl + "contacts?$select=fullname,emailaddress1&$filter=contains(emailaddress1,'" + email + "')");
            if (result?.value?.Count == 1)
            {
                crmCheckEmail.Message = "This email address is already exist.";
                crmCheckEmail.IsExisted = true;
                crmCheckEmail.Email = email;
                string fullname= result.value.Select(r => r.fullname).FirstOrDefault().ToString();
                crmCheckEmail.firstname = fullname.Split()[0];
                crmCheckEmail.lastname = fullname.Split()[1];
                //Console.WriteLine("result :" + result.value.Select(r => r.fullname).ToString());

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
