

using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
    
        public class UserRepository : IUserRepository
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<UserRepository> _logger;

            public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
            {
                _context = context;
                _logger = logger;
            }

            
            
            public async Task<List<User>> GetRecentlyVisitedUsersForGuestAsync(Guid guestId, int count)
            {
                return await (from User in _context.Users
                              join room in _context.Rooms on User.RoomId equals room.Id
                              join roomType in _context.RoomTypes on room.RoomTypeId equals roomType.Id
                              join User in _context.Users on roomType.UserId equals User.Id
                              where User.UserId == guestId
                              orderby User.CheckInDate descending
                              select User).Distinct().Take(count)
                    .ToListAsync();
            }

        public async Task InsertAsync(User user)
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

        public async Task<User> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _context.Users.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogError($"User with ID {id} was not found.");
                    throw new NotFoundException($"User with ID {id} was not found.");

                }
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching User by ID: {ex.Message}");
                throw new DataAccessException("An error occurred while retrieving the User.", ex);
            }
        }

        

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await GetByIdAsync(id);

                _context.Users.Remove(entity);
                _logger.LogInformation($"User deleted from the database");
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
                _logger.LogError($"Error deleting User: {ex.Message}");
                throw new DataAccessException("An error occurred while deleting the User.", ex);
            }
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            try
            {
                return await _context.Users.FindAsync(id) != null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking existence of entity: {ex.Message}");
                throw new DataAccessException("An error occurred while checking the existence of the entity.", ex);
            }
        }

        public async Task UpdateAsync(User User, Guid id)
        {
            try
            {
                if (User == null)
                    throw new ArgumentNullException(nameof(User), "Entity cannot be null.");

                var existingEntity = await GetByIdAsync(id);




                _context.Entry(existingEntity).CurrentValues.SetValues(User);

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating User: {ex.Message}");
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
