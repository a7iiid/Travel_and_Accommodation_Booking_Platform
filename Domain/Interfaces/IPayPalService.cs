
public interface IPayPalService
{
    Task<Order> CreateOrderAsync(decimal amount, string currency);
    Task<Order> CaptureOrderAsync(string orderId);
}