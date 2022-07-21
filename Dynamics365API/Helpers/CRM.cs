using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Dynamics365API.Helpers
{
    public class CRM
    {
        private readonly IConfiguration _configuration;
        public CRM(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> GetAccessTokenAsync()
        {
            var applicationId = _configuration["DYNAMICS365API:ApplicationId"];
            var clientSecret = _configuration["DYNAMICS365API:ClientSecret"];
            var organizationUrl = _configuration["DYNAMICS365API:OrganizationUrl"];
            var aadInstanceUrl = _configuration["DYNAMICS365API:AadInstanceUrl"];
            var tenantId = _configuration["DYNAMICS365API:TenantId"];

            var clientcred = new ClientCredential(applicationId, clientSecret);
            var authenticationContext = new AuthenticationContext($"{aadInstanceUrl}/{tenantId}");
            var authenticationResult = await authenticationContext.AcquireTokenAsync(organizationUrl, clientcred);
            return authenticationResult.AccessToken;
        }

        public async Task<HttpClient> GetD365ClientAsync()
        {
            var accessToken = await GetAccessTokenAsync();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            //client.DefaultRequestHeaders.Add("OData-Version", "4.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            //client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"*\"");
            
            return client;
        }

        public string GetOrganizationAPIUrl()
        {
            return _configuration["DYNAMICS365API:BaseURLAPI"];
        }
    }
}
