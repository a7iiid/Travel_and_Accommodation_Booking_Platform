﻿

using Application.DTOs.BookingDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.profile
{
    public class BookingProfile:Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDTO>();
            CreateMap<Booking, BookingByIdDTO>();
                
            CreateMap<BookingByIdDTO, Booking>()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<BookingDTO, Booking>();
            CreateMap<BookingDTOForCreation, Booking>()
                .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId))
                .ForMember(dest => dest.CheckInDate, opt => opt.MapFrom(src => src.CheckInDate))
                .ForMember(dest => dest.CheckOutDate, opt => opt.MapFrom(src => src.CheckOutDate))
                .ForMember(dest => dest.BookingDate, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore Id as it will be generated by the database
            CreateMap<BookingResultDTO, Booking>();
            CreateMap<Booking, BookingResultDTO>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Room, opt => opt.Ignore());

        }
    }
}
