using AutoMapper;
using Domain.Entities;
using Application.DTOs.HotelDTOs;

namespace Application.profile
{
    public class HotelProfile : Profile
    {
        public HotelProfile()
        {
            // Map Hotel -> HotelDTO
            CreateMap<Hotel, HotelDTO>();

            // Map HotelDTO -> Hotel
            CreateMap<HotelDTO, Hotel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore Id during updates
        }
    }
}
