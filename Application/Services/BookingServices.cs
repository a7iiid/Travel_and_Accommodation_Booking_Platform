using Domain.Entities;
using Application.DTOs.BookingDTOs;
using AutoMapper;
using Infrastructure.Repository;
using Domain.Enum;
using Domain.Exceptions;
using Application.DTOs.PaymentDTOs;
using Infrastructure.DB;
using Domain.Interfaces;

namespace Application.Services
{
    public class BookingServices
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IPaymentRepository _paymentServices;
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;
        public BookingServices(
            IBookingRepository bookingRepository,
            IRoomRepository roomRepository,
            IPaymentRepository paymentServices,
            IMapper mapper,
            ApplicationDbContext context)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _paymentServices = paymentServices;
            _mapper = mapper;
            _context = context; 
        }

        /// <summary>
        /// Gets all bookings.
        /// </summary>
        /// <returns>A list of BookingDTOs</returns>
        public async Task<IReadOnlyList<Booking>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.Any()
                ? bookings
                : throw new NotFoundException("No bookings found");
        }

        /// <summary>
        /// Gets a booking by its ID.
        /// </summary>
        /// <param name="bookingId">The ID of the booking.</param>
        /// <returns>A Booking</returns>
        public async Task<BookingByIdDTO?> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return null;
            }
            var bookingDto = _mapper.Map<BookingByIdDTO>(booking);
            return bookingDto;
        }

        /// <summary>
        /// Creates a new booking.
        /// </summary>
        /// <param name="bookingDto">The BookingDTO containing booking details.</param>
        /// <returns>The created BookingDTO</returns>
        public async Task<BookingResultDTO?> CreateBookingAsync(BookingDTOForCreation bookingDto,Guid userGuid)
        {

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var price = CalculateTotalPrice(bookingDto);

                // Create booking
                var booking = new Booking
                {
                    RoomId = bookingDto.RoomId,
                    UserId = userGuid,
                    CheckInDate = bookingDto.CheckInDate,
                    CheckOutDate = bookingDto.CheckOutDate,
                    Price = await price,
                    BookingDate = DateTime.UtcNow
                };

                // add booking
                var createdBooking = await _bookingRepository.InsertAsync(booking);
                if (createdBooking == null)
                {

                    return null;

                }

                // Create payment
                Payment payment = new Payment
                {
                    BookingId = createdBooking.Id,
                    Amount = await price,
                    Method = bookingDto.PaymentMethod,
                    Status = PaymentStatus.Pending
                };




                BookingResultDTO bookingResult = _mapper.Map<BookingResultDTO>(createdBooking);
                 var createOrderResult= await _paymentServices.InsertAsync(payment);
                bookingResult.ApproveLink = createOrderResult.ApprovalUrl;
                await transaction.CommitAsync();

                return bookingResult;

            }
            catch(Exception ex)
            {   
                transaction.Rollback();
                throw new InvalidOperationException("Failed to create booking");

            }


        }


        private async Task<double> GetRoomPriceAsync(Guid roomId)
        {
            var pricePerNight = await _roomRepository.GetRoomWithPriceAsync(roomId);
            return pricePerNight ?? throw new InvalidOperationException("Room not found");
        }
        private async Task <double> CalculateTotalPrice(BookingDTOForCreation bookingDto)
        {
            var nights = CalculateNights(bookingDto);
            double? pricePerNight = await GetRoomPriceAsync(bookingDto.RoomId);
           
            var totalPrice = pricePerNight * nights;
            return totalPrice??0;
        }
        private static int CalculateNights(BookingDTOForCreation bookingDto)
        {

            // Calculate price based on room type and duration
            var nights = (bookingDto.CheckOutDate.Date - bookingDto.CheckInDate.Date).Days;
            if (nights <= 0) throw new InvalidOperationException("Invalid date range");
            return nights;
        }

        /// <summary>
        /// Updates an existing booking.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to update.</param>
        /// <param name="updatedBookingDto">The updated BookingDTO.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public async Task<bool> UpdateBookingAsync(Guid bookingId, BookingForUpdateDTO updatedBookingDto)
        {
            var existingBooking = await _bookingRepository.GetByIdAsync(bookingId);
            if (existingBooking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {bookingId} was not found.");
            }

            _mapper.Map(updatedBookingDto, existingBooking);

            await _bookingRepository.UpdateAsync(existingBooking, bookingId);
            return true;

        }

        /// <summary>
        /// Deletes a booking by its ID.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to delete.</param>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
        public async Task<bool> DeleteBookingAsync(Guid bookingId)
        {
            var bookingExists = await _bookingRepository.IsExistsAsync(bookingId);
            if (!bookingExists)
            {
                throw new KeyNotFoundException($"Booking with ID {bookingId} does not exist.");
            }

            return await _bookingRepository.DeleteAsync(bookingId);
        }
    }
}
