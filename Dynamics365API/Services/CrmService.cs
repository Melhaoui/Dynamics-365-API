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
using Dynamics365API.Dtos;

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
            organizationAPIUrl += "contacts" +
                                  "?$select=fullname,emailaddress1" +
                                  "&$filter=contains(emailaddress1,'" + email + "')";
            var result = await httpClient.GetFromJsonAsync<CrmCheckEmailDataDto>(organizationAPIUrl);
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

        public async Task<CrmContactDto> GetContactAsync(string email)
        {
            CrmContactDto crmContactDto = new CrmContactDto { };
            var contact = await GetAccountByContactEmailAsync(email);
            var data = JObject.Parse(contact.ToString());
            if (data?.SelectToken("value[0]")?.ToString().Length > 0 )
            {
                crmContactDto.entityimage = data.SelectToken("value[0].entityimage").ToString();

                string contactId = data.SelectToken("value[0].contactid").ToString();
                string primaryContactId = data.SelectToken("value[0].parentcustomerid_account._primarycontactid_value").ToString();
                if (contactId == primaryContactId)
                    crmContactDto.isPrimary = true;
            } 

            return crmContactDto;
        }

        public async Task<string> GetAllEmailTeamAsync(string email, string queryEamil)
        {
            string contactId = await GetContactIdAsync(email);
            string accountId = await GetAccountIdAsync(contactId);

            var allEmailTeamNotPrimary = (dynamic)null;
            if (queryEamil== "emailaddress")
              allEmailTeamNotPrimary = await GetAllEmailTeamNotPrimaryAsync(accountId, null, queryEamil);
            else
                allEmailTeamNotPrimary = await GetAllEmailTeamNotPrimaryAsync(accountId, email, queryEamil);

            var allEmailTeamNotPrimaryFilter = string.Join(" or ", allEmailTeamNotPrimary);
            
            return allEmailTeamNotPrimaryFilter;
        }

        public async Task<CrmCrudDto> CrmCrud(HttpMethod httpMethod, string entityQuery, object body)
        {
            CrmCrudDto crudCrmDto = new()
            {
                Message = "Something is wrong",
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

        //--------------------------------- Dashboard ---------------------------------//
        public async Task<object> GetOpportunitiesStatusCodeCountAsync(string email)
        {
            var result = (dynamic)null;
            var contact = await GetAccountByContactEmailAsync(email);
            var data = JObject.Parse(contact.ToString());
            if (data?.SelectToken("value[0]")?.ToString().Length > 0)
            {
                string accountId = data.SelectToken("value[0].parentcustomerid_account.accountid").ToString();
                //string query = $"opportunities?$apply=groupby((statuscode),aggregate(opportunityid with countdistinct as count))" +
                //               $"&$filter=_parentaccountid_value eq '{accountId}'";
                string query = $"opportunities?$apply=groupby((statuscode),aggregate(opportunityid with countdistinct as count))";
                result = await GetEntityAsync(query);
            }
            return result;
        }


        public async Task<object> GetOpportunitiesEstmatedRevenueAsync(string email)
        {
            //var result = (dynamic)null;
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();
            
            var account = await GetAccountByContactEmailAsync(email);
            var data = JObject.Parse(account.ToString());
            if (data?.SelectToken("value[0]")?.ToString().Length > 0)
            {
                string accountId = data.SelectToken("value[0].parentcustomerid_account.accountid").ToString();
                organizationAPIUrl += $"opportunities" +
                                      $"?$select=estimatedclosedate,estimatedvalue" +
                                      $"&$filter=_parentaccountid_value eq '{accountId}'";

               var result = await httpClient.GetFromJsonAsync<CrmDataDto<CrmOpportunitiesRevenue>>(organizationAPIUrl);
                var query= result.Value.Where(e => e.estimatedclosedate.StartsWith(DateTime.Now.Year.ToString()))
                                        .GroupBy(p => p.estimatedclosedate,
                                                 p => p.estimatedvalue,
                                                 (key, g) => new { estimatedclosedate = key, estimatedvalue = g.Sum() })
                                        .OrderBy(p=>p.estimatedclosedate)
                                        .Select(e => new { x = e.estimatedclosedate, y = e.estimatedvalue, z = e.estimatedvalue })
                                        .ToList();



                return query;
            }

            return null;
        }

        private async Task<string> GetContactIdAsync(string email)
        {
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();
            organizationAPIUrl += $"contacts" +
                                  $"?$select=contactid" +
                                  $"&$filter=contains(emailaddress1, '{email}') ";
            var contact = await httpClient.GetFromJsonAsync<CrmDataDto<CrmTeamOpportunitiesValueDto>>(organizationAPIUrl);
            if (contact is null || contact?.Value?.Count == 0)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var contactId =  contact?.Value?.Select(c => c.ContactId)?.FirstOrDefault()?.ToString();

            return contactId;
        }

        private async Task<string> GetAccountIdAsync(string contactId)
        {
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();
            organizationAPIUrl += $"accounts" +
                                  $"?$select=accountid" +
                                  $"&$filter=primarycontactid/contactid eq {contactId} ";
            var account = await httpClient.GetFromJsonAsync<CrmDataDto<CrmTeamOpportunitiesValueDto>>(organizationAPIUrl);
            if (account is null || account?.Value?.Count == 0)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var accountId = account?.Value?.Select(c => c.AccountId)?.FirstOrDefault()?.ToString();

            return accountId;
        }

        private async Task<List<string>> GetAllEmailTeamNotPrimaryAsync(string accountId, string? email, string queryEamil)
        {
            var httpClient = await _crm.GetD365ClientAsync();
            string organizationAPIUrl = _crm.GetOrganizationAPIUrl();
            organizationAPIUrl +=$"contacts?$select=emailaddress1&$filter=_parentcustomerid_value eq {accountId} ";
            var allEmailTeam = await httpClient.GetFromJsonAsync<CrmDataDto<CrmTeamOpportunitiesAllEmailDto>>(organizationAPIUrl);
            var allEmailTeamNotPrimary = (dynamic)null;
            if(email==null)
                allEmailTeamNotPrimary = allEmailTeam.Value
                                        .Select(e => queryEamil + " eq '" + e.Emailaddress1 + "'")
                                        .ToList();
            else
                allEmailTeamNotPrimary = allEmailTeam.Value
                                        .Select(e => queryEamil + " eq '" + e.Emailaddress1 + "'")
                                        .Where(x => x != queryEamil+" eq '" + email + "'")
                                        .ToList();
            

            if (allEmailTeamNotPrimary is null || allEmailTeamNotPrimary?.Count == 0)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return allEmailTeamNotPrimary;
        }

        private async Task<object> GetAccountByContactEmailAsync(string email)
        {
            string query = $"contacts?$select=contactid, entityimage"+
                           $"&$filter=contains(emailaddress1, '{email}')"+
                           $"&$expand=parentcustomerid_account($select=_primarycontactid_value)";
            var result = await GetEntityAsync(query);

            return result;
        }
    }
}
