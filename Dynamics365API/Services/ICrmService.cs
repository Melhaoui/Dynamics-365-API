using Dynamics365API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dynamics365API.Services
{
    public interface ICrmService
    {
        Task<CrmCheckEmailDto> CheckEmailAsync(string email);

        Task<object> GetEntityAsync(string entityQuery);

        Task<object> GetTeamOpportunitiesAsync(string email);

        Task<string> AddEntityAsync(string entityQuery, object jsonObject);
    }
}