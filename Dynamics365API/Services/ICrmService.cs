using Dynamics365API.Dtos.Crm;
using Microsoft.AspNetCore.Mvc;

namespace Dynamics365API.Services
{
    public interface ICrmService
    {
        Task<CrmCheckEmailDto> CheckEmailAsync(string email);

        Task<object> GetEntityAsync(string entityQuery);

        Task<bool> GetContactIsPrimaryAsync(string email);

        Task<string> GetAllEmailTeamAsync(string email, string queryEamil);

        Task<string> AddEntityAsync(string entityQuery, object jsonObject);

        Task<object> CrmCrud(HttpMethod httpMethod, string requestUri, object body);
    }
}