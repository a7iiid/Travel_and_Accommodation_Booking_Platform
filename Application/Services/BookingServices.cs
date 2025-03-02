using Domain.Entities;
using Application.DTOs.BookingDTOs;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Model;

namespace Application.Services
{
    public class BookingServices
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;

        private readonly IMapper _mapper;
        public BookingServices(
            IBookingRepository bookingRepository,
            IRoomRepository roomRepository,
            IMapper mapper
            )
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _mapper = mapper;
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
        public async Task<BookingResult?> CreateBookingAsync(BookingDTOForCreation bookingDto,Guid userGuid)
        {

            var price =await CalculateTotalPrice(bookingDto);

            // Create booking
            var booking = new Booking
            {
                RoomId = bookingDto.RoomId,
                UserId = userGuid,
                CheckInDate = bookingDto.CheckInDate,
                CheckOutDate = bookingDto.CheckOutDate,
                Price =  price,
                BookingDate = DateTime.UtcNow
            };
            // add booking
            var createdBooking = await _bookingRepository.InsertAsync(booking);
            if (createdBooking == null)
            {

                return null;

            }
            return createdBooking;

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

        public async Task<PaginatedList<Hotel>> GetRecentlyVisitedHotelsAsync(Guid userId, int pageNumber, int pageSize)
        {
            return await _bookingRepository.GetRecentlyVisitedHotelsAsync(userId, pageNumber, pageSize);
        }

    }
}
