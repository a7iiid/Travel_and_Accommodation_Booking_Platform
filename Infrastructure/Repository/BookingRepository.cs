using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;


namespace Infrastructure.Repository
{
    public class BookingRepository : IBookingRepository 
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(ApplicationDbContext context, ILogger<BookingRepository> logger)
            
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

        public  async Task<IReadOnlyList<Booking>> GetAllAsync()
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

        public  async Task<Booking> GetByIdAsync(Guid id)
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
                    _logger.LogError($"Booking  with ID {id} was not found.");
                    throw new NotFoundException($"Booking with ID {id} was not found.");

                }
                return booking;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching entity by ID: {ex.Message}");
                throw new DataAccessException("An error occurred while retrieving the booking.", ex);
            }
        }

        public  async Task<Booking> InsertAsync(Booking booking)
        {
            if (!await CanBookRoom(booking.RoomId, booking.CheckInDate, booking.CheckOutDate))
            {
                _logger.LogWarning("Room not available for selected dates");
                throw new BookingConflictException("Room is already booked for the selected dates.");
            }

            await _context.AddAsync(booking);
            await SaveChangesAsync();
            return booking;
        }
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("saving changes");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving changes: {ex.Message}");
                throw new DataAccessException("An error occurred while saving changes to the database.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {

            try
            {
                var entity = await GetByIdAsync(id);

                _context.Bookings.Remove(entity);
                _logger.LogInformation($"Booking deleted from the database");
                await SaveChangesAsync();
                return true;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting entity: {ex.Message}");
                throw new DataAccessException("An error occurred while deleting the entity.", ex);
            }
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            try
            {
                return await _context
                            .Bookings
                            .AnyAsync
                            (booking => booking.Id.Equals(id));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking existence of Booking: {ex.Message}");
                throw new DataAccessException("An error occurred while checking the existence of the entity.", ex);
            }
        }

        public async Task UpdateAsync(Booking booking, Guid id)
        {
            try
            {
                if (booking == null)
                    throw new ArgumentNullException(nameof(booking), "Entity cannot be null.");

                var existingEntity = await GetByIdAsync(id);




                _context.Entry(existingEntity).CurrentValues.SetValues(booking);

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Booking: {ex.Message}");
                throw new DataAccessException("An error occurred while updating the entity.", ex);
            }
        }

        public async Task<PaginatedList<Hotel>> GetRecentlyVisitedHotelsAsync(Guid userId, int pageNumber, int pageSize)
        {
            try
            {
                var hotels =await (from booking in _context.Bookings
                                join room in _context.Rooms on booking.RoomId equals room.Id
                                join roomType in _context.RoomTypes on room.RoomTypeId equals roomType.Id
                                join hotel in _context.Hotels on roomType.HotelId equals hotel.Id
                                where booking.UserId == userId
                                orderby booking.CheckInDate descending
                                select hotel).Distinct()
                                .ToListAsync();

                PageData pageData = new PageData(hotels.Count, pageSize, pageNumber);

                return new PaginatedList<Hotel>(hotels, pageData);
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching recently visited hotels: {ex.Message}");
                throw new DataAccessException("An error occurred while fetching the recently visited hotels.", ex);
            }
        }
    }
}
