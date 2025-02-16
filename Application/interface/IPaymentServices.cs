using Application.DTOs.PaymentDTOs;

namespace Application.@interface
{
    public interface IPaymentServices
    {
        Task<string?> AddPaymentAsync(PaymentDTO payment);
    }
}
