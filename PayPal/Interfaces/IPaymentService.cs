
namespace Payment.Interfaces
{
    public interface IPaymentService
    {
       public Task<Domain.Entities.Payment> CreateOrderAsync(decimal amount, string currency);
       public Task<Domain.Entities.Payment> CaptureOrderAsync(string orderId);
    }
}
