
using Domain.Entities;
using Domain.Enum;
using Domain.Model;

namespace Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<CreateOrderResult?> InsertAsync(Payment payment);
        Task<bool> DeleteAsync(Guid id);
        Task<PaginatedList<Payment>> GetAllAsync( int pageNumber, int pageSize);
        Task<Payment> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
        Task UpdateAsync(Payment Payment, Guid id);
        Task<bool> UpdatePaymentStatusWebHookAsync(string orderId,PaymentStatus paymentStatus);
    }
}
