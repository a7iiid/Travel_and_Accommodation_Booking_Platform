using Application.DTOs.CityDTOs;
using Application.DTOs.HotelDTOs;
using Application.DTOs.RoomDTOs;
using Application.Services;
using Application.Validators;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : Controller
    {
        private readonly HotelServices _hotelService;

        public HotelController(HotelServices hotelService)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
        }

        /// <summary>
        /// Retrieves a paginated list of hotels.
        /// </summary>
        /// <param name="searchQuery">Search query for filtering hotels by name, description, or address.</param>
        /// <param name="pageNumber">Page number for pagination (default: 1).</param>
        /// <param name="pageSize">Number of items per page (default: 10).</param>
        /// <returns>Paginated list of hotels.</returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedList<HotelDTO>>> GetHotels(
            string? searchQuery = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var hotels = await _hotelService.GetHotelsAsync(searchQuery, pageNumber, pageSize);
            return Ok(hotels);
        }

        /// <summary>
        /// Retrieves a specific hotel by ID.
        /// </summary>
        /// <param name="id">The ID of the hotel.</param>
        /// <returns>The hotel with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDTO>> GetHotelById(Guid id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }

        /// <summary>
        /// Adds a new hotel to the database.
        /// </summary>
        /// <param name="hotelDTO">The hotel data transfer object containing the hotel details.</param>
        /// <returns>A confirmation message upon successful addition.</returns>
        [HttpPost]
      //  [Authorize("Admin")]
        public async Task<IActionResult> AddHotel(HotelDTO hotelDTO)
        {
            var validator = new HotelDTOValidator();
            var validationResult = await validator.ValidateAsync(hotelDTO);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }

            await _hotelService.AddHotelAsync(hotelDTO);
            return Ok("Hotel added successfully.");
        }

        /// <summary>
        /// Updates an existing hotel.
        /// </summary>
        /// <param name="id">The ID of the hotel to update.</param>
        /// <param name="hotelDTO">The updated hotel data transfer object.</param>
        /// <returns>No content on successful update.</returns>
        [HttpPut("{id}")]
      //  [Authorize("Admin")]
        public async Task<IActionResult> UpdateHotel(Guid id, HotelDTO hotelDTO)
        {
            var validator = new HotelDTOValidator();
            var validationResult = await validator.ValidateAsync(hotelDTO);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }

            await _hotelService.UpdateHotelAsync(id, hotelDTO);
           
            return NoContent();
        }

        /// <summary>
        /// Deletes a hotel by ID.
        /// </summary>
        /// <param name="id">The ID of the hotel to delete.</param>
        /// <returns>No content on successful deletion, or Not Found if the hotel does not exist.</returns>
        [HttpDelete("{id}")]
       // [Authorize("Admin")]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            var result = await _hotelService.DeleteHotelAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Searches for available rooms in a hotel within a specified date range.
        /// </summary>
        /// <param name="hotelId">The ID of the hotel.</param>
        /// <param name="checkInDate">Check-in date.</param>
        /// <param name="checkOutDate">Check-out date.</param>
        /// <returns>List of available rooms.</returns>
        [HttpGet("{hotelId}/available-rooms")]
        public async Task<ActionResult<List<RoomDTO>>> GetAvailableRooms(
            Guid hotelId,
            DateTime checkInDate,
            DateTime checkOutDate)
        {
            var rooms = await _hotelService.GetAvailableRoomsAsync(hotelId, checkInDate, checkOutDate);
            return Ok(rooms);
        }

    }
}
