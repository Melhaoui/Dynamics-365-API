using Dynamics365API.Dtos;
using Dynamics365API.Dtos.Crm;
using Microsoft.AspNetCore.Mvc;

namespace Dynamics365API.Services
{
    public interface ICrmService
    {
        Task<CrmCheckEmailDto> CheckEmailAsync(string email);

        Task<object> GetEntityAsync(string entityQuery);

        Task<CrmContactDto> GetContactAsync(string email);

        Task<string> GetAllEmailTeamAsync(string email, string queryEamil);

        Task<CrmCrudDto> CrmCrud(HttpMethod httpMethod, string requestUri, object body);

        //--------------------------------- Dashboard ---------------------------------//
        Task<object> GetOpportunitiesStatusCodeCountAsync(string email);

        Task<object> GetOpportunitiesEstmatedRevenueAsync(string email);
    }
}