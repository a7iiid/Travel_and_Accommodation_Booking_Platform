using Domain.Entities;
using Domain.Exceptions;
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

        public override async Task<Booking> GetByIdAsync(Guid id)
        {
            try
            {
                var booking =await _context.Bookings
                            .Include(b=> b.Review)
                            .Include(b=>b.Payment)
                            .Include(b=>b.Room)
                            .Include(b=>b.User)
                            
                            .FirstOrDefaultAsync(b => b.Id == id);
                if (booking == null)
                {
                    log.LogError($"Booking  with ID {id} was not found.");
                    throw new NotFoundException($"Booking with ID {id} was not found.");

                }
                return booking;

            }
            catch (Exception ex)
            {
                log.LogError($"Error fetching entity by ID: {ex.Message}");
                throw new DataAccessException("An error occurred while retrieving the booking.", ex);
            }
        }

        public override async Task<Booking> AddAsync(Booking booking)
        {
            if (!await CanBookRoom(booking.RoomId, booking.CheckInDate, booking.CheckOutDate))
            {
                _logger.LogWarning("Room not available for selected dates");
                throw new BookingConflictException("Room is already booked for the selected dates.");
            }

            await base.AddAsync(booking);
            return booking;
        }



    }
}
