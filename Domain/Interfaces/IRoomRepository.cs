using Domain.Entities;
using Domain.Model;

namespace Domain.Interfaces
{
    public interface IRoomRepository
    {
        Task<bool> CheckRoomBelongsToHotelAsync(Guid hotelId, Guid roomId);
        Task<bool> DeleteAsync(Guid id);
        Task<PaginatedList<Room>> GetAllAsync(string? searchQuery, int pageNumber, int pageSize);
        Task<Room> GetByIdAsync(Guid id);
        Task<PaginatedList<Room>> GetRoomsByHotelIdAsync(Guid hotelId, string? searchQuery, int pageNumber, int pageSize);
        Task<float?> GetRoomWithPriceAsync(Guid roomId);
        Task<Room?> GetRoomWithTypeAsync(Guid roomId);
        Task<bool> IsExistsAsync(Guid id);
        Task SaveChangesAsync();
        Task UpdateAsync(Room Room, Guid id);
    }
}
