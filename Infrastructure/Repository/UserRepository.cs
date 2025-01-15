

using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
    
        public class UserRepository : Repository<User>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<UserRepository> _logger;

            public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
                : base(context, logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Guid> GetGuestIdByEmailAsync(string email)
            {
                return (await _context
                    .Users
                    .Where
                    (user => user
                        .Email
                        .Equals(email))
                    .SingleAsync()).Id;
            }
            public override async Task AddUserAsync(User user)
            {
                try
                {
                    await _context.Users.AddAsync(user);
                    await SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException != null && e.InnerException.Message.Contains("Role"))
                    {
                    _logger.LogError("User already exists in the system.");
                        throw new UserAlreadyExistsException("User already exists in the system.");
                    }
                _logger.LogError("Error Adding User. Check for a violation of User attributes.");

                throw new DataConstraintViolationException("Error Adding User. Check for a violation of User attributes.");
                }
            }

            public async Task<List<Hotel>> GetRecentlyVisitedHotelsForGuestAsync(Guid guestId, int count)
            {
                return await (from booking in _context.Bookings
                              join room in _context.Rooms on booking.RoomId equals room.Id
                              join roomType in _context.RoomTypes on room.RoomTypeId equals roomType.Id
                              join hotel in _context.Hotels on roomType.HotelId equals hotel.Id
                              where booking.UserId == guestId
                              orderby booking.CheckInDate descending
                              select hotel).Distinct().Take(count)
                    .ToListAsync();
            }

            public async Task<List<Hotel>> GetRecentlyVisitedHotelsForAuthenticatedGuestAsync(string email, int count)
            {
                var guestId = await GetGuestIdByEmailAsync(email);
                return await GetRecentlyVisitedHotelsForGuestAsync(guestId, count);
            }

            public async Task<List<Booking>> GetBookingsForAuthenticatedGuestAsync(string email, int count)
            {
                var guestId = await GetGuestIdByEmailAsync(email);
                return await (from booking in _context.Bookings
                              where booking.UserId == guestId
                              orderby booking.CheckInDate descending
                              select booking)
                    .Take(count)
                    .ToListAsync();
            }


        }

    
}
