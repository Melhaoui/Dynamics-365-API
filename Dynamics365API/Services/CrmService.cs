using Dynamics365API.Dtos;
using Dynamics365API.Helpers;

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

        public async Task<string> GetEntity(string entityQuery)
        {
            string resultJson = null;
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();

            var httpResponse = await httpClient.GetAsync(organizationAPIUrl + entityQuery);

            if (httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    resultJson = httpResponse.Content.ReadAsStringAsync().Result;
                }
            }
            return resultJson;
        }

    }
}
