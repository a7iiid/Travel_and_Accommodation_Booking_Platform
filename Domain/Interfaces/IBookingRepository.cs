using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Booking>> GetAllAsync();
        Task<bool> CanBookRoom(Guid roomId, DateTime checkInDate, DateTime checkOutDate);
        Task<Booking> AddAsync(Booking booking);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> IsExistsAsync(Guid id);
        Task UpdateAsync(Booking booking, Guid id);
        Task SaveChangesAsync();
    }
}
