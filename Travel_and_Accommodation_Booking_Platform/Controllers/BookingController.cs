using Application.DTOs.BookingDTOs;
using Application.Services;
using Application.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Ensure the user is authenticated
    public class BookingController : ControllerBase
    {
        private readonly BookingServices _bookingServices;

        public BookingController(BookingServices bookingServices)
        {
            _bookingServices = bookingServices;
        }

        /// <summary>
        /// Get all bookings
        /// </summary>
        /// <returns>List of bookings</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingServices.GetAllBookingsAsync();
            return Ok(bookings);
        }

        /// <summary>
        /// Get a booking by ID
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>The booking details</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            var booking = await _bookingServices.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound($"Booking with ID {id} not found.");

            return Ok(booking);
        }

        /// <summary>
        /// Create a new booking
        /// </summary>
        /// <param name="bookingDto">Booking details</param>
        /// <returns>Created booking</returns>
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDTOForCreation bookingDto)
        {
            var userId = User.FindFirst("Id")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            bookingDto.UserId = Guid.Parse(userId);

            var createdBooking = await _bookingServices.CreateBookingAsync(bookingDto);
            if (createdBooking == null)
                return BadRequest("The room is unavailable for the selected dates.");

            return CreatedAtAction(nameof(GetBookingById), new { id = createdBooking.UserId }, createdBooking);
        
    }

        /// <summary>
        /// Update an existing booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="updatedBookingDto">Updated booking details</param>
        /// <returns>Updated booking</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] BookingDTO updatedBookingDto)
        {
            // Validate the DTO
            var validator = new BookingDTOValidator();
            var validationResult = await validator.ValidateAsync(updatedBookingDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }

            // Get the current user's email
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("User email not found.");

            // Check if the booking exists and belongs to the current user
            var booking = await _bookingServices.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound($"Booking with ID {id} not found.");

            if (booking.User.Email != userEmail)
                return Forbid("You are not authorized to update this booking.");

            try
            {
                var isUpdated = await _bookingServices.UpdateBookingAsync(id, updatedBookingDto);
                if (!isUpdated)
                    return NotFound($"Booking with ID {id} not found.");

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a booking by ID
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Delete status</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            // Get the current user's email
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("User email not found.");

            // Check if the booking exists and belongs to the current user
            var booking = await _bookingServices.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound($"Booking with ID {id} not found.");

            if (booking.User.Email != userEmail)
                return Forbid("You are not authorized to delete this booking.");

            try
            {
                var isDeleted = await _bookingServices.DeleteBookingAsync(id);
                if (!isDeleted)
                {
                    return NotFound($"Booking with ID {id} not found.");
                }

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}