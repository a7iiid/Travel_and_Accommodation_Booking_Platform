using Application.DTOs.CityDTOs;
using Application.Services;
using Application.Validators;
using Domain.Entities;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Validetors.cityValidetors;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CitiesController : ControllerBase
    {
        private readonly CityServices _cityService;
       
        public CitiesController(CityServices cityService)
        {
            _cityService = cityService ?? throw new ArgumentNullException(nameof(cityService));
        }

        /// <summary>
        /// Retrieves a paginated list of cities without associated hotels.
        /// </summary>
        /// <param name="searchQuery">Optional search query for filtering cities by name or country.</param>
        /// <param name="pageNumber">Page number for pagination (default: 1).</param>
        /// <param name="pageSize">Number of items per page (default: 10).</param>
        /// <returns>Paginated list of cities without hotels.</returns>
        [HttpGet("without-hotels")]
        public async Task<ActionResult<PaginatedList<CityDTOWithoutHotels>>> GetCitiesWithoutHotels(
            string? searchQuery = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var cities = await _cityService.GetCitiesWithOutHotelsAsync(searchQuery, pageNumber, pageSize);
            return Ok(cities);
        }

        /// <summary>
        /// Retrieves a paginated list of cities with their associated hotels.
        /// </summary>
        /// <param name="searchQuery">Optional search query for filtering cities by name or country.</param>
        /// <param name="pageNumber">Page number for pagination (default: 1).</param>
        /// <param name="pageSize">Number of items per page (default: 10).</param>
        /// <returns>Paginated list of cities with hotels.</returns>
        [HttpGet("with-hotels")]
        public async Task<ActionResult<PaginatedList<CityDTO>>> GetCitiesWithHotels(
            string? searchQuery = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var cities = await _cityService.GetCitiesWithHotelsAsync(searchQuery, pageNumber, pageSize);
            return Ok(cities);
        }

        /// <summary>
        /// Retrieves a city by its unique ID.
        /// </summary>
        /// <param name="id">Unique identifier of the city.</param>
        /// <returns>The city with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCityById(Guid id)
        {
            var city = await _cityService.GetCityByIdAsync(id);
            return Ok(city);
        }

        /// <summary>
        /// Adds a new city to the database.
        /// </summary>
        /// <param name="cityDTO">The city data transfer object containing the city details.</param>
        /// <returns>A confirmation message upon successful addition.</returns>
        [HttpPost]
        [Authorize("Admin")]
        public async Task<IActionResult> AddCity([FromBody] CityDTOForAdd cityDTO)
        {
            var validator = new CityDTOForAddValidetor();
            var validationResult = await validator.ValidateAsync(cityDTO);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }

            await _cityService.AddCityAsync(cityDTO);
            return Ok("City added successfully.");
        }


        /// <summary>
        /// Updates an existing city.
        /// </summary>
        /// <param name="id">Unique identifier of the city to update.</param>
        /// <param name="cityDTO">The updated city data transfer object.</param>
        /// <returns>204 No Content on successful update.</returns>
        [HttpPut("{id}")]
        [Authorize("Admin")]

        public async Task<IActionResult> UpdateCity(Guid id, CityDTO cityDTO)
        {
            // Validate the cityDTO
            var validator = new CityDTOValidator();

            var validationResult = await validator.ValidateAsync(cityDTO);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }

            await _cityService.UpdateCityAsync(cityDTO, id);
            return NoContent();
        }
        /// <summary>
        /// Deletes a city by its unique ID.
        /// </summary>
        /// <param name="id">Unique identifier of the city to delete.</param>
        /// <returns>204 No Content on successful deletion, or 404 Not Found if the city does not exist.</returns>
        [HttpDelete("{id}")]
        [Authorize("Admin")]

        public async Task<IActionResult> DeleteCity(Guid id)
        {
            var result = await _cityService.DeleteCityAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
