
using PayPalCheckoutSdk.Orders;

namespace Pay.Interfaces
{
    public interface IPayment
    {
       public Task<Order> CreateOrderAsync(decimal amount, string currency);
       public Task<Order> CaptureOrderAsync(string orderId);
    }
}
