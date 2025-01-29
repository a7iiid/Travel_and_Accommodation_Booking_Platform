using Domain.Entities;
using Domain.Interfaces;
using Application.DTOs.BookingDTOs;
using AutoMapper;
using Infrastructure.Repository;

namespace Application.Services
{
    public class BookingServices
    {
        private readonly BookingRepository _bookingRepository;
        private readonly RoomRepository _roomRepository;
        private readonly PaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        public BookingServices(
            BookingRepository bookingRepository,
            RoomRepository roomRepository,
            PaymentRepository paymentRepository,
            IMapper mapper)
            {
                _bookingRepository = bookingRepository;
                _roomRepository = roomRepository;
                _paymentRepository = paymentRepository;
                _mapper = mapper;
            }

        /// <summary>
        /// Gets all bookings.
        /// </summary>
        /// <returns>A list of BookingDTOs</returns>
        public async Task<IReadOnlyList<Booking>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings;
        }

        /// <summary>
        /// Gets a booking by its ID.
        /// </summary>
        /// <param name="bookingId">The ID of the booking.</param>
        /// <returns>A Booking</returns>
        public async Task<Booking?> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            return booking == null ? null :booking;
        }

        /// <summary>
        /// Creates a new booking.
        /// </summary>
        /// <param name="bookingDto">The BookingDTO containing booking details.</param>
        /// <returns>The created BookingDTO</returns>
        public async Task<BookingDTO?> CreateBookingAsync(BookingDTOForCreation bookingDto)
        {
            // Validate room availability
            if (!await _bookingRepository.CanBookRoom(
                bookingDto.RoomId,
                bookingDto.CheckInDate,
                bookingDto.CheckOutDate))
            {
                throw new InvalidOperationException("The selected room is not available for the given dates.");
            }

            // Map DTO to entity
            var booking = _mapper.Map<Booking>(bookingDto);

            // Add booking to the repository
            var createdBooking = await _bookingRepository.AddAsync(booking);
            return createdBooking == null ? null : _mapper.Map<BookingDTO>(createdBooking);
        }

        /// <summary>
        /// Updates an existing booking.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to update.</param>
        /// <param name="updatedBookingDto">The updated BookingDTO.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public async Task<bool> UpdateBookingAsync(Guid bookingId, BookingDTO updatedBookingDto)
        {
            var existingBooking = await _bookingRepository.GetByIdAsync(bookingId);
            if (existingBooking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {bookingId} was not found.");
            }

            // Update entity fields
            var updatedBooking = _mapper.Map<Booking>(updatedBookingDto);
            await _bookingRepository.UpdateAsync(updatedBooking, bookingId);
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
