using Domain.Entities;
using Domain.Model;


namespace Domain.Interfaces
{
    public interface IRoomTypeRepository
    {
        Task<bool> CheckRoomTypeExistenceForHotel(Guid hotelId, Guid roomTypeId);
        Task<bool> DeleteAsync(Guid id);
        Task<PaginatedList<RoomType>> GetAllAsync(Guid hotelId, bool includeAmenities, int pageNumber, int pageSize);
        Task<RoomType> GetByIdAsync(Guid id);
        Task<bool> IsExistsAsync(Guid id);
        Task SaveChangesAsync();
        Task UpdateAsync(RoomType RoomType, Guid id);
    }

}
