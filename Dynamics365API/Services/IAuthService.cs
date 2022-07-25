using System.Threading.Tasks;
using Dynamics365API.Dtos;
using Dynamics365API.Models;
using Microsoft.AspNetCore.Identity;

namespace Dynamics365API.Services
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto model);

        Task<AuthDto> GetTokenAsync(TokenRequestDto model);

        Task<ApplicationUser> GetUserByEmailAsync(string email);

        Task<IdentityResult> ConfirmEmailAsync(string uid, string token);

        Task GenerateEmailConfirmationTokenAsync(ApplicationUser user);

        Task GenerateForgotPasswordTokenAsync(ApplicationUser user);

        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model);
    }
}