using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace EmailService
{
    public class SuccessfulEmailService
    {
        private readonly IConfiguration _configuration;

        public SuccessfulEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }




    }
}
