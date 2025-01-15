using Domain.Entities;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking> 
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(ApplicationDbContext context, ILogger<BookingRepository> logger)
            : base(context, logger) 
        {
            _context = context;
            _logger = logger;
        }

    
        public async Task<bool> CanBookRoom(Guid roomId, DateTime proposedCheckIn, DateTime proposedCheckOut)
        {
            var roomBookings = await _context
                .Bookings
                .Where(b => b.RoomId.Equals(roomId))
                .ToListAsync();

            return roomBookings.All(booking =>
                proposedCheckIn.Date > booking.CheckOutDate.Date ||
                proposedCheckOut.Date < booking.CheckInDate.Date);
        }

        public override async Task<IReadOnlyList<Booking>> GetAllAsync()
        {
            try
            {
                return await _context
                    .Bookings
                    .Include(booking => booking.Payment)
                    .Include(booking => booking.Review)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception)
            {
                return Array.Empty<Booking>();
            }
        }

       

        public override async Task<Booking?> AddUserAsync(Booking booking)
        {
            if (!await CanBookRoom(
                    booking.RoomId,
                    booking.CheckInDate,
                    booking.CheckOutDate)) return null;

            await base.AddUserAsync(booking);
            return booking;
        }

       
    }
}
