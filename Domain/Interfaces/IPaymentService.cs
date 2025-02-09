

using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> CreateOrderAsync(decimal amount, string currency);
        Task<Payment> CaptureOrderAsync(string orderId);
    }
}
