
using Domain.Entities;
using Infrastructure.Auth.model;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> DeleteAsync(Guid id);
        Task<User> GetByIdAsync(Guid id);
        Task<List<Hotel>> GetRecentlyVisitedHotelAsync(Guid guestId, int count);
        Task InsertAsync(User user);
        Task<bool> IsExistsAsync(Guid id);
        Task SaveChangesAsync();
        Task UpdateAsync(User User, Guid id);
        public Task<UserResultModel?> GetUserByEmailAsync(string email);


    }
}
