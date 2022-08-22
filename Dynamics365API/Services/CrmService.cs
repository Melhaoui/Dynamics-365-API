using Dynamics365API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Web.Http;
using Dynamics365API.Dtos.Crm;
using System.Collections.Generic;

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
            }

            return crmCheckEmail;
        }

        public async Task<object> GetEntityAsync(string entityQuery)
        {
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();

            var result = await httpClient.GetFromJsonAsync<object>(organizationAPIUrl + entityQuery);

            return result;
        }

        public async Task<bool> GetContactIsPrimaryAsync(string email)
        {
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();
            var contact = await httpClient.GetFromJsonAsync<object>(organizationAPIUrl + $"contacts?$select=contactid&$filter=contains(emailaddress1, '{email}')&$expand=parentcustomerid_account($select=_primarycontactid_value)");
            var data = JObject.Parse(contact.ToString());
            if (data?.SelectToken("value[0]")?.ToString().Length > 0 )
            {
                string contactId = data.SelectToken("value[0].contactid").ToString();
                string primaryContactId = data.SelectToken("value[0].parentcustomerid_account._primarycontactid_value").ToString();
                if (contactId == primaryContactId)
                    return true;
            } 

            return false;
        }

        public async Task<object> GetTeamOpportunitiesAsync(string email)
        {
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();

            string contactId = await GetContactAsync(email);
            string accountId = await GetAccountAsync(contactId);

            var allEmailTeamNotPrimary = await GetAllEmailTeamNotPrimaryAsync(accountId, email);
            var allEmailTeamNotPrimaryFilter = string.Join(" or ", allEmailTeamNotPrimary);
            var result = await httpClient.GetFromJsonAsync<object>(organizationAPIUrl + $"opportunities?$select=name,emailaddress,totalamount,actualclosedate,estimatedclosedate,actualvalue,closeprobability&$filter={allEmailTeamNotPrimaryFilter} ");

            return result;
        }

        public async Task<object> CrmCrud(HttpMethod httpMethod, string entityQuery, object body)
        {
            CrmCrudDto crudCrmDto = new()
            {
                Message = "Something is wrong",
                Data = body,
            };

            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();

            httpClient.BaseAddress = new Uri(organizationAPIUrl);
            JObject jsonBody = JObject.FromObject(body);
            HttpRequestMessage updateRequest1 = new HttpRequestMessage(httpMethod, entityQuery)
            {
                Content = new StringContent(jsonBody.ToString(), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage updateResponse1 = await httpClient.SendAsync(updateRequest1);
            if (updateResponse1.StatusCode == HttpStatusCode.NoContent)
                crudCrmDto.Message = "successfully";

            return crudCrmDto;
        }

        private async Task<string> GetContactAsync(string email)
        {
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();

            var contact = await httpClient.GetFromJsonAsync<CrmTeamOpportunitiesDataDto<CrmTeamOpportunitiesValueDto>>(organizationAPIUrl + $"contacts?$select=contactid&$filter=contains(emailaddress1, '{email}') ");
            if (contact is null || contact?.Value?.Count == 0)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var contactId =  contact?.Value?.Select(c => c.ContactId)?.FirstOrDefault()?.ToString();

            return contactId;
        }

        private async Task<string> GetAccountAsync(string contactId)
        {
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();

            var account = await httpClient.GetFromJsonAsync<CrmTeamOpportunitiesDataDto<CrmTeamOpportunitiesValueDto>>(organizationAPIUrl + $"accounts?$select=accountid&$filter=primarycontactid/contactid eq {contactId} ");
            if (account is null || account?.Value?.Count == 0)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var accountId = account?.Value?.Select(c => c.AccountId)?.FirstOrDefault()?.ToString();

            return accountId;
        }

        private async Task<List<string>> GetAllEmailTeamNotPrimaryAsync(string accountId, string email)
        {
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();

            var allEmailTeam = await httpClient.GetFromJsonAsync<CrmTeamOpportunitiesDataDto<CrmTeamOpportunitiesAllEmailDto>>(organizationAPIUrl + $"contacts?$select=emailaddress1&$filter=_parentcustomerid_value eq {accountId} ");
            var allEmailTeamNotPrimary = allEmailTeam.Value.Select(e => "emailaddress eq '" + e.Emailaddress1 + "'").Where(x => x != "emailaddress eq '" + email + "'").ToList();

            if (allEmailTeamNotPrimary is null || allEmailTeamNotPrimary?.Count == 0)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return allEmailTeamNotPrimary;
        }

        //------------------------------AddEntity Not working------------------------------------------------------------------
        public async Task<string> AddEntityAsync(string entityQuery,object jsonObject)
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
