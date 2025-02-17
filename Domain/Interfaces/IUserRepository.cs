
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<Hotel>> GetRecentlyVisitedHotelsForGuestAsync(Guid guestId, int count);


    }
}
