using System.Threading.Tasks;
using Dynamics365API.Dtos;

namespace Dynamics365API.Services
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto model);
        Task<AuthDto> GetTokenAsync(TokenRequestDto model);
    }
}