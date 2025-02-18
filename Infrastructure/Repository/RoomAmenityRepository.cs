

using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
   

    public class RoomAmenityRepository : IRoomAmenityRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoomAmenityRepository> _logger;

        public RoomAmenityRepository(ApplicationDbContext context, ILogger<RoomAmenityRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedList<RoomAmenity>> GetAllAsync(string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.RoomAmenities.AsQueryable();
                var totalItemCount = await query.CountAsync();
                var pageData = new PageData(totalItemCount, pageSize, pageNumber);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                    query = query.Where
                    (city => city.Name.Contains(searchQuery) ||
                    city.Description.Contains(searchQuery));
                }

                var result = query
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToList();

                return new PaginatedList<RoomAmenity>(result, pageData);
            }
            catch (Exception)
            {
                _logger.LogWarning("Data Access Exception");
                return new PaginatedList<RoomAmenity>(new List<RoomAmenity>(), new PageData(0, 0, 0));
            }
        }
        public async Task InsertAsync(RoomAmenity RoomAmenity)
        {
            try
            {
                await _context.RoomAmenities.AddAsync(RoomAmenity);
                await SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null && e.InnerException.Message.Contains("Role"))
                {
                    _logger.LogError("RoomAmenity already exists in the system.");
                    throw new RoomAmenityAlreadyExistsException("RoomAmenity already exists in the system.");
                }
                _logger.LogError("Error Adding RoomAmenity. Check for a violation of RoomAmenity attributes.");

                throw new DataConstraintViolationException("Error Adding RoomAmenity. Check for a violation of RoomAmenity attributes.");
            }
        }

        public async Task<RoomAmenity> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _context.RoomAmenities.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogError($"RoomAmenity with ID {id} was not found.");
                    throw new NotFoundException($"RoomAmenity with ID {id} was not found.");

                }
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching RoomAmenity by ID: {ex.Message}");
                throw new DataAccessException("An error occurred while retrieving the RoomAmenity.", ex);
            }
        }



        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await GetByIdAsync(id);

                _context.RoomAmenities.Remove(entity);
                _logger.LogInformation($"RoomAmenity deleted from the database");
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
                _logger.LogError($"Error deleting RoomAmenity: {ex.Message}");
                throw new DataAccessException("An error occurred while deleting the RoomAmenity.", ex);
            }
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            try
            {
                return await _context.RoomAmenities.FindAsync(id) != null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking existence of entity: {ex.Message}");
                throw new DataAccessException("An error occurred while checking the existence of the entity.", ex);
            }
        }

        public async Task UpdateAsync(RoomAmenity RoomAmenity, Guid id)
        {
            try
            {
                if (RoomAmenity == null)
                    throw new ArgumentNullException(nameof(RoomAmenity), "Entity cannot be null.");

                var existingEntity = await GetByIdAsync(id);




                _context.Entry(existingEntity).CurrentValues.SetValues(RoomAmenity);

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating RoomAmenity: {ex.Message}");
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
