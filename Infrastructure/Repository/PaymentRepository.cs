
using Domain.Entities;
using Domain.Enum;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.DB;
using Infrastructure.EmailService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pay.Interfaces;
using System.Security.Claims;

namespace Infrastructure.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentRepository> _logger;
        private readonly IPaymentGateway _paymentGateway;
        public PaymentRepository(ApplicationDbContext context
                                    ,IPaymentGateway payment
                                ,ILogger<PaymentRepository>logger,
                                IEmailSender emailSender)
        {
            _context= context ?? throw new ArgumentNullException(nameof(context));
            _paymentGateway = payment ?? throw new ArgumentNullException(nameof(payment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender=emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            
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
                var paymentResult = await _paymentGateway.CreateOrderAsync((decimal)payment.Amount, "USD");

                payment.ApprovalUrl = paymentResult.ApprovalUrl;
                payment.OrderId=paymentResult.OrderId;
                payment.Method=paymentResult.PaymentMethod;
                await _context.Payments.AddAsync(payment);
                
                await SaveChangesAsync();

                return paymentResult;
            }
            catch(Exception ex) 
                {     
                  _logger.LogError($"Error inserting Payment: {ex.Message}");
                  throw new DataAccessException("An error occurred while inserting the entity.", ex);
                }
        }

        public async Task UpdatePaymentStatusWebHookAsync(string orderId,PaymentStatus paymentStatus)
        {
            _logger.LogInformation($"Received PayPal Webhook");

            try
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
                
                if(paymentStatus == PaymentStatus.Completed)
                {
                    var booking = await _context
                                    .Bookings.Include(b => b.User)
                                    .FirstOrDefaultAsync(b => b.Id == payment.BookingId);
                    var email = new Email
                    {
                        Amount = payment.Amount,
                        BookingId = payment.BookingId,
                        Name = booking.User.FirstName + " " + booking.User.LastName,
                        ToEmail = booking.User.Email


                    };
                    await _emailSender.SendEmail(email);

                }
                if (payment != null)
                {
                    payment.Status = paymentStatus;
                    await SaveChangesAsync();
                    _logger.LogInformation($"Payment {orderId} marked as COMPLETED.");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error processing PayPal Webhook: {ex.Message}");

            }

        }

    }
}
