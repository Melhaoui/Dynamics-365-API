using Dynamics365API.Dtos;

namespace Dynamics365API.Services
{
    public interface IEmailService
    {
        Task SendTestEmail(UserEmailOptionsDto userEmailOptionsDto);

        Task SendEmailForEmailConfirmation(UserEmailOptionsDto userEmailOptionsDto);

        Task SendEmailForForgotPassword(UserEmailOptionsDto userEmailOptionsDto);

    }
}