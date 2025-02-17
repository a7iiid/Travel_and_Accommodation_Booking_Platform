using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repository
{
   

    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoomRepository> _logger;

        public RoomRepository(ApplicationDbContext context, ILogger<RoomRepository> logger)

        {
            _context = context;
            _logger = logger;
        }


        public async Task<Room> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _context.Rooms.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogError($"Room with ID {id} was not found.");
                    throw new NotFoundException($"Room with ID {id} was not found.");

                }
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Room by ID: {ex.Message}");
                throw new DataAccessException("An error occurred while retrieving the Room.", ex);
            }
        }



        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await GetByIdAsync(id);

                _context.Rooms.Remove(entity);
                _logger.LogInformation($"Room deleted from the database");
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
                _logger.LogError($"Error deleting Room: {ex.Message}");
                throw new DataAccessException("An error occurred while deleting the Room.", ex);
            }
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            try
            {
                return await _context.Rooms.FindAsync(id) != null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking existence of entity: {ex.Message}");
                throw new DataAccessException("An error occurred while checking the existence of the entity.", ex);
            }
        }

        public async Task UpdateAsync(Room Room, Guid id)
        {
            try
            {
                if (Room == null)
                    throw new ArgumentNullException(nameof(Room), "Entity cannot be null.");

                var existingEntity = await GetByIdAsync(id);




                _context.Entry(existingEntity).CurrentValues.SetValues(Room);

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Room: {ex.Message}");
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
        private async Task<PaginatedList<Room>> PaginateQueryAsync(IQueryable<Room> query, int pageNumber, int pageSize)
        {
            var totalItemCount = await query.CountAsync();
            var pageData = new PageData(totalItemCount, pageSize, pageNumber);

            var result = await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PaginatedList<Room>(result, pageData);
        }

        public async Task<PaginatedList<Room>> GetAllAsync(string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Rooms.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                }
                return await PaginateQueryAsync(query, pageNumber, pageSize);
            }
            catch (Exception)
            {
                return new PaginatedList<Room>(new List<Room>(), new PageData(0, 0, 0));
            }
        }

        public async Task<PaginatedList<Room>> GetRoomsByHotelIdAsync(Guid hotelId, string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                var query = (from roomType in _context.RoomTypes
                             join room in _context.Rooms on roomType.Id equals room.RoomTypeId
                             where roomType.HotelId == hotelId
                             select room);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                }

                return await PaginateQueryAsync(query, pageNumber, pageSize);
            }
            catch (Exception)
            {
                return new PaginatedList<Room>(new List<Room>(), new PageData(0, 0, 0));
            }
        }

        public async Task<bool> CheckRoomBelongsToHotelAsync(Guid hotelId, Guid roomId)
        {
            return await (from roomType in _context.RoomTypes
                          where roomType.HotelId.Equals(hotelId)
                          join room in _context.Rooms on
                          roomType.Id equals room.RoomTypeId
                          where room.Id.Equals(roomId)
                          select room)
                .AnyAsync();
        }

        public async Task<Room?> GetRoomWithTypeAsync(Guid roomId)
        {
            return await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }
        public async Task<float?> GetRoomWithPriceAsync(Guid roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            return room?.RoomType?.PricePerNight;
        }



    }

}
