
using Domain.Model;

namespace Pay.Interfaces
{
    public interface IPaymentGateway
    {
       public Task<CreateOrderResult> CreateOrderAsync(decimal amount, string currency);
    }
}
