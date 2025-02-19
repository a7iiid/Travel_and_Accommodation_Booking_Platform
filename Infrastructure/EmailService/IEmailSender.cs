using Domain.Model;

namespace Infrastructure.EmailService
{
    public interface IEmailSender
    {
        Task SendEmail(Email email);
    }
}