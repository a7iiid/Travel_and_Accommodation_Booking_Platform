using Domain.Entities;
using Domain.Model;

namespace Domain.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> DeleteAsync(Guid id);
        Task<PaginatedList<Review>> GetAllByHotelIdAsync(Guid hotelId, string? searchQuery, int pageNumber, int pageSize);
        Task<Review> GetByIdAsync(Guid id);
        Task InsertAsync(Review Review);
        Task<bool> IsExistsAsync(Guid id);
        Task SaveChangesAsync();
        Task UpdateAsync(Review Review, Guid id);
    }
}
