using Domain.Entities;
using Domain.Model;

namespace Domain.Interfaces
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task<bool> CheckRoomBelongsToHotelAsync(Guid hotelId, Guid roomId);
        Task<PaginatedList<Room>> GetAllAsync(string? searchQuery, int pageNumber, int pageSize);
        Task<PaginatedList<Room>> GetRoomsByHotelIdAsync(Guid hotelId, string? searchQuery, int pageNumber, int pageSize);
        Task<float?> GetRoomWithPriceAsync(Guid roomId);
        Task<Room?> GetRoomWithTypeAsync(Guid roomId);
    }
}
