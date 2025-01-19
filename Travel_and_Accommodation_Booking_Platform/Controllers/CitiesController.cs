using Application.Services;
using Domain.Entities;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet]
        public async Task<ActionResult<PaginatedList<City>>> GetCities(
           bool includeHotels = false,
           string? searchQuery = null,
           int pageNumber = 1,
           int pageSize = 10)
        {
            var cities = await _cityService.GetCitiesAsync(includeHotels, searchQuery, pageNumber, pageSize);
            return Ok(cities);
        }
    }
}
