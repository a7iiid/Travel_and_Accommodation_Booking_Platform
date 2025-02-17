
using Domain.Entities;
using Domain.Model;

namespace Domain.Interfaces
{
    public interface IRoomAmenityRepository
    {
        Task<bool> DeleteAsync(Guid id);
        Task<PaginatedList<RoomAmenity>> GetAllAsync(string? searchQuery, int pageNumber, int pageSize);
        Task<RoomAmenity> GetByIdAsync(Guid id);
        Task InsertAsync(RoomAmenity RoomAmenity);
        Task<bool> IsExistsAsync(Guid id);
        Task SaveChangesAsync();
        Task UpdateAsync(RoomAmenity RoomAmenity, Guid id);
    }
}
