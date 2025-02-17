

using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
   
    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoomTypeRepository> _logger;

        public RoomTypeRepository(ApplicationDbContext context, ILogger<RoomTypeRepository> logger)

        {
            _context = context;
            _logger = logger;
        }


        public async Task<RoomType> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _context.RoomTypes.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogError($"RoomType with ID {id} was not found.");
                    throw new NotFoundException($"RoomType with ID {id} was not found.");

                }
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching RoomType by ID: {ex.Message}");
                throw new DataAccessException("An error occurred while retrieving the RoomType.", ex);
            }
        }



        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await GetByIdAsync(id);

                _context.RoomTypes.Remove(entity);
                _logger.LogInformation($"RoomType deleted from the database");
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
                _logger.LogError($"Error deleting RoomType: {ex.Message}");
                throw new DataAccessException("An error occurred while deleting the RoomType.", ex);
            }
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            try
            {
                return await _context.RoomTypes.FindAsync(id) != null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking existence of entity: {ex.Message}");
                throw new DataAccessException("An error occurred while checking the existence of the entity.", ex);
            }
        }

        public async Task UpdateAsync(RoomType RoomType, Guid id)
        {
            try
            {
                if (RoomType == null)
                    throw new ArgumentNullException(nameof(RoomType), "Entity cannot be null.");

                var existingEntity = await GetByIdAsync(id);




                _context.Entry(existingEntity).CurrentValues.SetValues(RoomType);

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating RoomType: {ex.Message}");
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
        public async Task<PaginatedList<RoomType>> GetAllAsync(Guid hotelId, bool includeAmenities, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context
                    .RoomTypes
                    .Where(roomType => roomType
                        .HotelId
                        .Equals(hotelId))
                    .AsQueryable();

                var totalItemCount = await query.CountAsync();
                var pageData = new PageData(totalItemCount, pageSize, pageNumber);

                if (includeAmenities)
                {
                    query = query.
                        Include(roomCategory =>
                        roomCategory.Amenities);
                }

                var result = query
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToList();

                return new PaginatedList<RoomType>(result, pageData);
            }
            catch (Exception)
            {
                return new PaginatedList<RoomType>(
                    new List<RoomType>(),
                    new PageData(0, 0, 0));
            }
        }



        public async Task<bool> CheckRoomTypeExistenceForHotel(Guid hotelId, Guid roomTypeId)
        {
            return (await GetByIdAsync(roomTypeId))
                   .HotelId.Equals(hotelId);
        }
    }

}
