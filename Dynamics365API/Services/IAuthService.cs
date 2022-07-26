﻿using System.Threading.Tasks;
using Dynamics365API.Dtos;
using Dynamics365API.Models;
using Microsoft.AspNetCore.Identity;

namespace Dynamics365API.Services
{
    public interface IAuthService
    {
        Task<bool> UpdateUser(string FirstName, string LastName, string Email);
        Task<AuthDto> RegisterAsync(RegisterDto model);

        Task<AuthDto> GetTokenAsync(TokenRequestDto model);

        Task<ApplicationUser> GetCurrentUserAsync(IHttpContextAccessor httpContextAccessor);

        Task<ApplicationUser> GetUserByIdAsync(string id);

        Task<ApplicationUser> GetUserByEmailAsync(string email);
        

        Task<IdentityResult> ConfirmEmailAsync(string uid, string token);

        Task GenerateEmailConfirmationTokenAsync(ApplicationUser user);

        Task GenerateForgotPasswordTokenAsync(ApplicationUser user);

        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model);
    }
}