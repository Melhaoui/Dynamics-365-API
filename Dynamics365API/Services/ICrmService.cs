using Dynamics365API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dynamics365API.Services
{
    public interface ICrmService
    {
        Task<CrmCheckEmailDto> CheckEmailAsync(string email);

        Task<object> GetEntity(string entityQuery);

        Task<string> AddEntity(string entityQuery, object jsonObject);
    }
}