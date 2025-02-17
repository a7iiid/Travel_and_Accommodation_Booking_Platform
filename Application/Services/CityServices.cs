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
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;


        public CityServices(ICityRepository cityRepository,IMapper mapper)
        {
            _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        public async Task<PaginatedList<CityDTO>> GetCitiesWithHotelsAsync(
            string? searchQuery,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentException("Page number and size must be greater than zero.");
            PaginatedList < City > cities = await ((CityRepository)_cityRepository).GetAllAsync(
                                                    true,
                                                    searchQuery,
                                                    pageNumber,
                                                    pageSize);
            var cityDto= _mapper.Map<List<CityDTO>>(cities.Items);
            return new PaginatedList<CityDTO>(cityDto, cities.PageData);

            
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

        public async Task<City> GetCityByIdAsync(Guid cityId,bool incloudHotel=false)
        {
            var city = await _cityRepository.GetByIdAsync(cityId, incloudHotel);
            if (city == null)
                throw new KeyNotFoundException($"City with ID {cityId} not found.");

            return city;
        }
        public async Task AddCityAsync(CityDTOForAdd cityDTO)
        {
            if (cityDTO == null)
                throw new ArgumentNullException(nameof(cityDTO));

            var cityEntity = _mapper.Map<City>(cityDTO);

            if (string.IsNullOrWhiteSpace(cityEntity.PostOfficeCode))
                throw new ArgumentException("PostOfficeCode cannot be null or empty.");

            await _cityRepository.InsertAsync(cityEntity);
        }

        public async Task UpdateCityAsync(CityDTO cityDTO, Guid id)
        {
            if (cityDTO == null)
                throw new ArgumentNullException(nameof(cityDTO));

            
            var existingCity = await _cityRepository.GetByIdAsync(id,false);
            if (existingCity == null)
                throw new KeyNotFoundException($"City with ID {id} not found.");

            
            _mapper.Map(cityDTO, existingCity);

            
            await _cityRepository.UpdateAsync(existingCity);
        }


        public async Task<bool> DeleteCityAsync(Guid cityId)
        {
            return await _cityRepository.DeleteAsync(cityId);
        }
    }
}
