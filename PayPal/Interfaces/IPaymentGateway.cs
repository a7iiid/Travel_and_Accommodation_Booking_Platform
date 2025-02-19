
using Domain.Model;
using PayPalCheckoutSdk.Orders;

namespace Pay.Interfaces
{
    public interface IPaymentGateway
    {
       public Task<CreateOrderResult> CreateOrderAsync(decimal amount, string currency);
       public Task<Order> GetOrderStatusAsync(string orderId);
    }
}
