using Dynamics365API.Dtos.Crm;
using Microsoft.AspNetCore.Mvc;

namespace Dynamics365API.Services
{
    public interface ICrmService
    {
        Task<CrmCheckEmailDto> CheckEmailAsync(string email);

        Task<object> GetEntityAsync(string entityQuery);

        Task<bool> GetContactIsPrimaryAsync(string email);

        Task<object> GetTeamOpportunitiesAsync(string email);

        Task<string> AddEntityAsync(string entityQuery, object jsonObject);

        Task<object> CrmCrud(HttpMethod httpMethod, string requestUri, object body);
    }
}