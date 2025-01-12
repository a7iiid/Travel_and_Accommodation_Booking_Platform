

using Infrastructure.Auth.model;

namespace Infrastructure.Auth
{
    public interface IAuthUser
    {
        public Task<User?> GetUserAsync(string email);
    }
}
