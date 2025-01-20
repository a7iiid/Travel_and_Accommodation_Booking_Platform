using Application.DTOs.CityDTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.Repository;

namespace Application.Services
{
    public class CityServices
    {
        private readonly IRepository<City> _cityRepository;
        private readonly IMapper _mapper;


        public CityServices(IRepository<City> cityRepository,IMapper mapper)
        {
            _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        public async Task<PaginatedList<City>> GetCitiesWithHotelsAsync(
            string? searchQuery,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentException("Page number and size must be greater than zero.");

            return await ((CityRepository)_cityRepository).GetAllAsync(
                true,
                searchQuery,
                pageNumber,
                pageSize);
        }
        public async Task<PaginatedList<CityDTOWithoutHotels>> GetCitiesWithOutHotelsAsync(
         string? searchQuery,
         int pageNumber,
         int pageSize)
        {
            var cities = await ((CityRepository)_cityRepository).GetAllAsync(false, searchQuery, pageNumber, pageSize);
            var cityDTOs = _mapper.Map<List<CityDTOWithoutHotels>>(cities.Items);
            return new PaginatedList<CityDTOWithoutHotels>(cityDTOs, cities.PageData);
        }
        public async Task<City> GetCityByIdAsync(Guid cityId)
        {
            var city = await _cityRepository.GetByIdAsync(cityId);
            if (city == null)
                throw new KeyNotFoundException($"City with ID {cityId} not found.");

            return city;
        }
        public async Task AddCityAsync(CityDTO city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            var cityDTOs = _mapper.Map<City>(city);
            await _cityRepository.AddAsync(cityDTOs);
        }
        public async Task UpdateCityAsync(CityDTO cityDTO, Guid id)
        {
            if (cityDTO == null)
                throw new ArgumentNullException(nameof(cityDTO));

            
            var existingCity = await _cityRepository.GetByIdAsync(id);
            if (existingCity == null)
                throw new KeyNotFoundException($"City with ID {id} not found.");

            
            _mapper.Map(cityDTO, existingCity);

            
            await _cityRepository.UpdateAsync(existingCity, id);
        }


        public async Task<bool> DeleteCityAsync(Guid cityId)
        {
            return await _cityRepository.DeleteAsync(cityId);
        }
    }
}
