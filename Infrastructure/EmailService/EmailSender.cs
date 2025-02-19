using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Domain.Model;

namespace Infrastructure.EmailService
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }
        public async Task SendEmail(Email email)
        {
            var fromEmail = _configuration["Email:FromEmail"];
            var fromPassword = _configuration["Email:FromPassword"];
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);

            using (var smtpClient = new SmtpClient(smtpHost,smtpPort))
            {


                smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
                smtpClient.EnableSsl = true;
                
                var subject = "Payment Successful";

                var body = $@"
            <h1>Payment Successful</h1>
            <p>Dear {email.FirstName},</p>
            <p>Your payment for booking ID {email.BookingId} has been successfully processed.</p>
            <p>Amount: {email.Amount:C}</p>
            <p>Thank you for choosing our platform!</p>
        ";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email.ToEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }




    }
}
