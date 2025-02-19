using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Email
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }
        public async Task SendEmail(string toEmail, string subject, string body)
        {
            var fromEmail = _configuration["Email:FromEmail"];
            var fromPassword = _configuration["Email:FromPassword"];
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = _configuration["Email:SmtpPort"];

            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = int.Parse(smtpPort),
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }




    }
}
