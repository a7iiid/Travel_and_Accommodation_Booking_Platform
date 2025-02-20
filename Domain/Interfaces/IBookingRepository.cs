using Domain.Entities;
using Domain.Model;

namespace Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Booking>> GetAllAsync();
        Task<bool> CanBookRoom(Guid roomId, DateTime checkInDate, DateTime checkOutDate);
        Task<Booking> InsertAsync(Booking booking);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> IsExistsAsync(Guid id);
        Task UpdateAsync(Booking booking, Guid id);
        Task<PaginatedList<Hotel>> GetRecentlyVisitedHotelsAsync(Guid userId, int pageNumber, int pageSize);

        Task SaveChangesAsync();
    }
}
