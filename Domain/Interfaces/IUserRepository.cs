
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> DeleteAsync(Guid id);
        Task<User> GetByIdAsync(Guid id);
        Task<List<Hotel>> GetRecentlyVisitedUsersForGuestAsync(Guid guestId, int count);
        Task InsertAsync(User user);
        Task<bool> IsExistsAsync(Guid id);
        Task SaveChangesAsync();
        Task UpdateAsync(User User, Guid id);
    }
}
