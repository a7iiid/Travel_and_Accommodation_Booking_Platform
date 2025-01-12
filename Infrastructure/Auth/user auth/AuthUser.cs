using Infrastructure.Auth.model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Auth
{
    public class AuthUser : IAuthUser
    {
        private readonly ApplicationDbContext _context;

        public AuthUser(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserAsync(string email)
        {
            var user = await _context
                .Users
                .SingleOrDefaultAsync(u => u.Email.Equals(email));

            if (user is null)
            {
                return null;
            }
            return new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.PasswordHash,
                IsAdmin = user.isAdmin,
                Salt = user.Salt
            };
        }
    }
}
