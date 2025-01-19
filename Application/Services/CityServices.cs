

using Domain.Entities;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.Repository;

namespace Application.Services
{
    public class CityServices
    {
        private readonly IRepository<City> _cityRepository;

        public CityServices(IRepository<City> cityRepository)
        {
            _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
        }

        public async Task<PaginatedList<City>> GetCitiesAsync(
            bool includeHotels,
            string? searchQuery,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentException("Page number and size must be greater than zero.");

            return await ((CityRepository)_cityRepository).GetAllAsync(
                includeHotels,
                searchQuery,
                pageNumber,
                pageSize);
        }
    }
}
