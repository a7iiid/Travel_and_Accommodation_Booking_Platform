using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Domain.Model;
using Infrastructure.Invoice;

namespace Infrastructure.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IInvoice _invoice;


        public EmailSender(IConfiguration configuration, IInvoice invoice)
        {
            _configuration = configuration;
            _invoice = invoice;
        }
        public async Task SendEmail(Email email)
        {

            var fromEmail =Environment.GetEnvironmentVariable("Email") ;
            var fromPassword = Environment.GetEnvironmentVariable("FromPassword");
            var smtpHost = Environment.GetEnvironmentVariable("SmtpHost");
            var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SmtpPort"));

            var pdfInvoice = _invoice.GenerateInvoiceAsync(email);

            using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
            {

                smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
                smtpClient.EnableSsl = true;

                var subject = "Payment Successful";

                var body = $@"
            <h1>Payment Successful</h1>
            <p>Dear {email.Name},</p>
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

                var pdfAttachment = new Attachment(new MemoryStream(pdfInvoice), "Invoice.pdf", "application/pdf");
                mailMessage.Attachments.Add(pdfAttachment);

                mailMessage.To.Add(email.ToEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }




    }
}
