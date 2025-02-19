
using Domain.Model;
using PayPalCheckoutSdk.Orders;

namespace Pay.Interfaces
{
    public interface IPayment
    {
       public Task<CreateOrderResult> CreateOrderAsync(decimal amount, string currency);
       public Task<Order> CaptureOrderAsync(string orderId);
    }
}
