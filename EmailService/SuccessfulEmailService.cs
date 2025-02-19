using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Domain.Model;

namespace EmailService
{
    public class SuccessfulEmailService
    {
        private readonly IConfiguration _configuration;

        public SuccessfulEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }

        public async Task SendEmailAsync(Email emailDto)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var smtpPort = int.Parse(emailSettings["SmtpPort"]);
            var smtpUsername = emailSettings["SmtpUsername"];
            var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            var fromEmail = emailSettings["FromEmail"];
            var fromName = emailSettings["FromName"];

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                var subject = "Payment Successful";

                var body = $@"
            <h1>Payment Successful</h1>
            <p>Dear {emailDto.FirstName},</p>
            <p>Your payment for booking ID {emailDto.BookingId} has been successfully processed.</p>
            <p>Amount: {emailDto.Amount:C}</p>
            <p>Thank you for choosing our platform!</p>
        ";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(emailDto.ToEmail);

                await client.SendMailAsync(mailMessage);
            }
        }




        }
}
