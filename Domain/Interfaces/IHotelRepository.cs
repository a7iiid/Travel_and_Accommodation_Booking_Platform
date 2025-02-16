using Domain.Entities;
using Domain.Model;


namespace Domain.Interfaces
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        Task<PaginatedList<Hotel>> GetAllAsync(string? searchQuery, int pageNumber, int pageSize);
        Task<List<Room>> GetHotelAvailableRoomsAsync(Guid hotelId, DateTime checkIn, DateTime checkOut);
        Task<PaginatedList<HotelSearchResult>> HotelSearchAsync(HotelSearchParameters searchParams);
        Task<PaginatedList<Hotel>> GetHotelsByOwnerIdAsync(Guid ownerId, int PageSize = 5, int PageNumber = 1);
    }
}
