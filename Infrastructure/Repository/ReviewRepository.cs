using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repository
{
   

    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReviewRepository> _logger;

        public ReviewRepository(ApplicationDbContext context, ILogger<ReviewRepository> logger)

        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedList<Review>> GetAllByHotelIdAsync(Guid hotelId, string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                var query = (from booking in _context.Bookings
                             join room in _context.Rooms on booking.RoomId equals room.Id
                             join roomType in _context.RoomTypes on room.RoomTypeId equals roomType.Id
                             join hotel in _context.Hotels on roomType.HotelId equals hotel.Id
                             join review in _context.Reviews on booking.Id equals review.BookingId
                             where roomType.HotelId == hotelId
                             select review)
                        .AsQueryable()
                        .AsNoTracking();

                var totalItemCount = await query.CountAsync();
                var pageData = new PageData(totalItemCount, pageSize, pageNumber);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                    query = query.Where
                        (review => review.Comment.Contains(searchQuery));
                }

                var result = query
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToList();

                return new PaginatedList<Review>(result, pageData);
            }
            catch (Exception)
            {
                _logger.LogWarning("Data Access Exception ");
                return new PaginatedList<Review>(new List<Review>(),
                new PageData(0, 0, 0));
            }
        }

        public async Task InsertAsync(Review Review)
        {
            try
            {
                await _context.Reviews.AddAsync(Review);
                await SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null && e.InnerException.Message.Contains("Role"))
                {
                    _logger.LogError("Review already exists in the system.");
                    throw new ReviewAlreadyExistsException("Review already exists in the system.");
                }
                _logger.LogError("Error Adding Review. Check for a violation of Review attributes.");

                throw new DataConstraintViolationException("Error Adding Review. Check for a violation of Review attributes.");
            }
        }

        public async Task<Review> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _context.Reviews.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogError($"Review with ID {id} was not found.");
                    throw new NotFoundException($"Review with ID {id} was not found.");

                }
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Review by ID: {ex.Message}");
                throw new DataAccessException("An error occurred while retrieving the Review.", ex);
            }
        }



        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await GetByIdAsync(id);

                _context.Reviews.Remove(entity);
                _logger.LogInformation($"Review deleted from the database");
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
                _logger.LogError($"Error deleting Review: {ex.Message}");
                throw new DataAccessException("An error occurred while deleting the Review.", ex);
            }
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            try
            {
                return await _context.Reviews.FindAsync(id) != null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking existence of entity: {ex.Message}");
                throw new DataAccessException("An error occurred while checking the existence of the entity.", ex);
            }
        }

        public async Task UpdateAsync(Review Review, Guid id)
        {
            try
            {
                if (Review == null)
                    throw new ArgumentNullException(nameof(Review), "Entity cannot be null.");

                var existingEntity = await GetByIdAsync(id);




                _context.Entry(existingEntity).CurrentValues.SetValues(Review);

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Review: {ex.Message}");
                throw new DataAccessException("An error occurred while updating the entity.", ex);
            }
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











    }
}
