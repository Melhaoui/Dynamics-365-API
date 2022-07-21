using Dynamics365API.Dtos;

namespace Dynamics365API.Services
{
    public interface ICrmService
    {
        Task<CrmCheckEmailDto> CheckEmailAsync(string email);

        Task<string> GetEntity(string entityQuery);
    }
}