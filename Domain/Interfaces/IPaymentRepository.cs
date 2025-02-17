
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<string?> InsertAsync(Payment payment);
    }
}
