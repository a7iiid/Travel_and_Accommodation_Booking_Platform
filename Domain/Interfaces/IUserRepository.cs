
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository:IRepository<User>
    {
        public Task<List<Hotel>> GetRecentlyVisitedHotelsForGuestAsync(Guid guestId, int count);


    }
}
