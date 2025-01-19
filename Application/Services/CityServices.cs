

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
        public async Task<City> GetCityByIdAsync(Guid cityId)
        {
            var city = await _cityRepository.GetByIdAsync(cityId);
            if (city == null)
                throw new KeyNotFoundException($"City with ID {cityId} not found.");

            return city;
        }
        public async Task AddCityAsync(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            await _cityRepository.AddUserAsync(city);
        }
        public async Task UpdateCityAsync(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            await _cityRepository.UpdateAsync(city);
        }

        public async Task<bool> DeleteCityAsync(Guid cityId)
        {
            return await _cityRepository.DeleteAsync(cityId);
        }
    }
}
