using Dynamics365API.Dtos;
using Dynamics365API.Helpers;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Dynamics365API.Services
{
    public class EmailService : IEmailService
    {
        private const string templatePath = @"EmailTemplate/{0}.html";
        private readonly SMTP _smtp;

        public EmailService(IOptions<SMTP> smtp)
        {
            _smtp = smtp.Value;
        }

        public async Task SendTestEmail(UserEmailOptionsDto userEmailOptionsDto)
        {
            userEmailOptionsDto.Subject = UpdatePlaceHolders("Hello {{UserName}}, This is test email subject from book store web app", userEmailOptionsDto.PlaceHolders);

            userEmailOptionsDto.Body = UpdatePlaceHolders(GetEmailBody("TestEmail"), userEmailOptionsDto.PlaceHolders);

            await SendEmail(userEmailOptionsDto);
        }

        public async Task SendEmailForEmailConfirmation(UserEmailOptionsDto userEmailOptionsDto)
        {
            userEmailOptionsDto.Subject = UpdatePlaceHolders("Hello {{UserName}}, Confirm your email id.", userEmailOptionsDto.PlaceHolders);

            userEmailOptionsDto.Body = UpdatePlaceHolders(GetEmailBody("EmailConfirm"), userEmailOptionsDto.PlaceHolders);

            await SendEmail(userEmailOptionsDto);
        }

        public async Task SendEmailForForgotPassword(UserEmailOptionsDto userEmailOptionsDto)
        {
            userEmailOptionsDto.Subject = UpdatePlaceHolders("Hello {{UserName}}, reset your password.", userEmailOptionsDto.PlaceHolders);

            userEmailOptionsDto.Body = UpdatePlaceHolders(GetEmailBody("ForgotPassword"), userEmailOptionsDto.PlaceHolders);

            await SendEmail(userEmailOptionsDto);
        }

        private async Task SendEmail(UserEmailOptionsDto userEmailOptionsDto)
        {
            MailMessage mail = new MailMessage
            {
                Subject = userEmailOptionsDto.Subject,
                Body = userEmailOptionsDto.Body,
                From = new MailAddress(_smtp.SenderAddress, _smtp.SenderDisplayName),
                IsBodyHtml = _smtp.IsBodyHTML
            };

            foreach (var toEmail in userEmailOptionsDto.ToEmails)
            {
                mail.To.Add(toEmail);
            }

            NetworkCredential networkCredential = new NetworkCredential(_smtp.UserName, _smtp.Password);

            SmtpClient smtpClient = new SmtpClient
            {
                Host = _smtp.Host,
                Port = _smtp.Port,
                EnableSsl = _smtp.EnableSSL,
                UseDefaultCredentials = _smtp.UseDefaultCredentials,
                Credentials = networkCredential,
            };

            mail.BodyEncoding = Encoding.Default;

            smtpClient.Send(mail);
        }

        private string GetEmailBody(string templateName)
        {
            var body = File.ReadAllText(string.Format(templatePath, templateName));
            return body;
        }

        private string UpdatePlaceHolders(string text, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!string.IsNullOrEmpty(text) && keyValuePairs != null)
            {
                foreach (var placeholder in keyValuePairs)
                {
                    if (text.Contains(placeholder.Key))
                    {
                        text = text.Replace(placeholder.Key, placeholder.Value);
                    }
                }
            }

            return text;
        }
    }
}
