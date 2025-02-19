
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pay.Interfaces;

namespace Infrastructure.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentRepository> _logger;
        private readonly IPayment _payment;
        public PaymentRepository(ApplicationDbContext context,IPayment payment,ILogger<PaymentRepository>logger)
        {
            _context= context ?? throw new ArgumentNullException(nameof(context));
            _payment = payment ?? throw new ArgumentNullException(nameof(payment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
        }
        public async Task<Payment> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _context.Payments.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogError($"Payment with ID {id} was not found.");
                    throw new NotFoundException($"Payment with ID {id} was not found.");

                }
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Payment by ID: {ex.Message}");
                throw new DataAccessException("An error occurred while retrieving the Payment.", ex);
            }
        }



        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await GetByIdAsync(id);

                _context.Payments.Remove(entity);
                _logger.LogInformation($"Payment deleted from the database");
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
                _logger.LogError($"Error deleting Payment: {ex.Message}");
                throw new DataAccessException("An error occurred while deleting the Payment.", ex);
            }
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            try
            {
                return await _context.Payments.FindAsync(id) != null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking existence of entity: {ex.Message}");
                throw new DataAccessException("An error occurred while checking the existence of the entity.", ex);
            }
        }

        public async Task UpdateAsync(Payment Payment, Guid id)
        {
            try
            {
                if (Payment == null)
                    throw new ArgumentNullException(nameof(Payment), "Entity cannot be null.");

                var existingEntity = await GetByIdAsync(id);




                _context.Entry(existingEntity).CurrentValues.SetValues(Payment);

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Payment: {ex.Message}");
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
        public async Task<PaginatedList<Payment>> GetAllAsync( int pageNumber, int pageSize)
        {
            try
            {
                var query = _context
                    .Payments
                    .AsQueryable();

                var totalItemCount = await query.CountAsync();
                var pageData = new PageData(totalItemCount, pageSize, pageNumber);

               

                var result = query
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToList();

                return new PaginatedList<Payment>(result, pageData);
            }
            catch (Exception)
            {
                return new PaginatedList<Payment>(
                    new List<Payment>(),
                    new PageData(0, 0, 0));
            }
        }

        public async Task<CreateOrderResult> InsertAsync(Payment payment)
        {
            try
            {
                var paymentResult = await _payment.CreateOrderAsync((decimal)payment.Amount, "USD");
                
                await _context.Payments.AddAsync(payment);

                return paymentResult;
            }
            catch(Exception ex) 
                {     
                  _logger.LogError($"Error inserting Payment: {ex.Message}");
                  throw new DataAccessException("An error occurred while inserting the entity.", ex);
                }
        }
    }
}
