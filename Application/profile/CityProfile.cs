using Application.DTOs.CityDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.profile
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            // Map City -> CityDTO
            CreateMap<City, CityDTO>()
               .ForMember(dest => dest.Hotels, opt => opt.MapFrom(src => src.Hotels));

            // Map CityDTO -> City
            CreateMap<CityDTO, City>()
               .ForMember(dest => dest.Hotels, opt => opt.Ignore())
               .ForMember(dest => dest.Id, opt => opt.Ignore()); ;

            // Map City -> CityDTOWithoutHotels
            CreateMap<City, CityDTOWithoutHotels>()
                      .ForMember(dest => dest.PostOffice, opt => opt.MapFrom(src => src.PostOfficeCode));

            // Map CityDTOWithoutHotels -> City
            
            CreateMap<CityDTOWithoutHotels, City>()
                .ForMember(dest => dest.PostOfficeCode, opt => opt.MapFrom(src => src.PostOffice));
            // Map City -> CityDTOForAdd

            CreateMap<City, CityDTOForAdd>()
                      .ForMember(dest => dest.PostOffice, opt => opt.MapFrom(src => src.PostOfficeCode));

            // Map CityDTOForAdd -> City

            CreateMap<CityDTOForAdd, City>()
                .ForMember(dest => dest.PostOfficeCode, opt => opt.MapFrom(src => src.PostOffice));
            

        }
    }
}
