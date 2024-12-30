﻿

using Infrastructure.DB;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{


    public class Repository<T> : ICRUDRepository<T> where T : class//this for the compiler know t is class for using with context
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger<Repository<T>> log;

        public Repository(ApplicationDbContext context, ILogger<Repository<T>> logger)
        {
            _context = context;
            log = logger;
        }



        public async Task<IEnumerable<T>> GetAll()
        {
            try
            {
                var result = await _context.Set<T>().ToListAsync();
                if (result == null || !result.Any())
                {
                    throw new NotFoundException("No records found.");
                }
                return result;
            }
            catch (NotFoundException ex)
            {
                log.LogWarning(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                log.LogError($"An error occurred while retrieving data: {ex.Message}");
                throw new DataAccessException("Failed to retrieve data from the database.", ex);
            }
        }


        public async Task Add(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                log.LogError($"Database update failed: {ex.Message}");
                throw new DataAccessException("Failed to add the entity to the database.", ex);
            }
            catch (Exception ex)
            {
                log.LogError($"An error occurred while adding the entity: {ex.Message}");
                throw new DataAccessException("An unexpected error occurred during the add operation.", ex);
            }
        }


        public async Task<T> GetById(Guid id)
        {
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity == null)
                    throw new NotFoundException($"Entity of type {typeof(T).Name} with ID {id} was not found.");
                return entity;
            }
            catch (Exception ex)
            {
                log.LogError($"Error fetching entity by ID: {ex.Message}");
                throw new DataAccessException("An error occurred while retrieving the entity.", ex);
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity == null)
                    throw new NotFoundException($"Entity of type {typeof(T).Name} with ID {id} was not found.");

                _context.Set<T>().Remove(entity);
                await SaveChangesAsync();
                return true;
            }
            catch (NotFoundException ex)
            {
                log.LogWarning(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                log.LogError($"Error deleting entity: {ex.Message}");
                throw new DataAccessException("An error occurred while deleting the entity.", ex);
            }
        }

        public async Task<bool> Exists(Guid id)
        {
            try
            {
                return await _context.Set<T>().FindAsync(id) != null;
            }
            catch (Exception ex)
            {
                log.LogError($"Error checking existence of entity: {ex.Message}");
                throw new DataAccessException("An error occurred while checking the existence of the entity.", ex);
            }
        }

        public async Task Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");

                _context.Set<T>().Update(entity);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                log.LogError($"Error updating entity: {ex.Message}");
                throw new DataAccessException("An error occurred while updating the entity.", ex);
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                log.LogError($"Error saving changes: {ex.Message}");
                throw new DataAccessException("An error occurred while saving changes to the database.", ex);
            }
        }


    }
}
