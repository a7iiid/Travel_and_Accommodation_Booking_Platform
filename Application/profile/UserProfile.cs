using Application.DTOs;
using AutoMapper;
using Domain.Entities;


namespace Application.profile
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {

            CreateMap<UserRegisterDTO, User>()
         .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
         .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
         .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
         .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
         .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

        }
    }
}
