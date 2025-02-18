using Domain.Entities;
using Domain.Model;


namespace Domain.Interfaces
{
    public interface IHotelRepository 
    {

        Task<Hotel> GetByIdAsync(Guid id);
        Task<Hotel> InsertAsync(Hotel hotel);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> IsExistsAsync(Guid id);
        Task UpdateAsync(Hotel hotel, Guid id);
        Task SaveChangesAsync();

        Task<PaginatedList<Hotel>> GetAllAsync(string? searchQuery, int pageNumber, int pageSize);
        Task<List<Room>> GetHotelAvailableRoomsAsync(Guid hotelId, DateTime checkIn, DateTime checkOut);
        Task<PaginatedList<HotelSearchResult>> HotelSearchAsync(HotelSearchParameters searchParams);
        Task<PaginatedList<Hotel>> GetHotelsByOwnerIdAsync(Guid ownerId, int PageSize = 5, int PageNumber = 1);
    }
}
