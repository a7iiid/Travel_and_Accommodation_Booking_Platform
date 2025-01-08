using Domain.Entities;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repository
{
    public class PaymentRepository : Repository<Payment>
    {
        

        public PaymentRepository(ApplicationDbContext context, ILogger<PaymentRepository> logger)
            :base(context,logger)
        {
            
        }

  
    }
}
