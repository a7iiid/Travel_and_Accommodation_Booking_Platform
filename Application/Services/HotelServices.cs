

using Application.DTOs.HotelDTOs;
using Application.DTOs.RoomDTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.Repository;

namespace Application.Services
{
    public class HotelServices
    {
        private readonly IMapper _mapper;
        private readonly HotelRepository _customHotelRepository;


        public HotelServices(HotelRepository hotelRepository, IMapper mapper)
        {
            _customHotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginatedList<HotelDTO>> GetHotelsAsync(string? searchQuery, int pageNumber, int pageSize)
        {

            var hotels = await _customHotelRepository.GetAllAsync(searchQuery, pageNumber, pageSize);
            var hotelDTOs = _mapper.Map<List<HotelDTO>>(hotels.Items);

            return new PaginatedList<HotelDTO>(hotelDTOs, hotels.PageData);
        }

        public async Task<List<RoomDTO>> GetAvailableRoomsAsync(Guid hotelId, DateTime checkInDate, DateTime checkOutDate)
        {

            var rooms = await _customHotelRepository.GetHotelAvailableRoomsAsync(hotelId, checkInDate, checkOutDate);
            return _mapper.Map<List<RoomDTO>>(rooms);
        }

        public async Task<PaginatedList<HotelSearchResult>> SearchHotelsAsync(HotelSearchParameters searchParams)
        {

            var searchResults = await _customHotelRepository.HotelSearchAsync(searchParams);
            var searchResultDTOs = _mapper.Map<List<HotelSearchResult>>(searchResults.Items);

            return new PaginatedList<HotelSearchResult>(searchResultDTOs, searchResults.PageData);
        }

        public async Task AddHotelAsync(HotelDTO hotelDTO)
        {

            var hotelEntity = _mapper.Map<Hotel>(hotelDTO);
            await _customHotelRepository.AddAsync(hotelEntity);
        }

        public async Task UpdateHotelAsync(Guid id, HotelDTO hotelDTO)
        {

            var existingHotel = await _customHotelRepository.GetByIdAsync(id);
            if (existingHotel == null)
            {
                throw new KeyNotFoundException($"Hotel with ID {id} not found.");
            }

            _mapper.Map(hotelDTO, existingHotel);
            await _customHotelRepository.UpdateAsync(existingHotel, id);
        }

        public async Task<bool> DeleteHotelAsync(Guid id)
        {

            return await _customHotelRepository.DeleteAsync(id);
        }
    }
}
