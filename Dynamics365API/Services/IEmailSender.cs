using Dynamics365API.Dtos;

namespace Dynamics365API.Services
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
