

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

            CreateMap<BookingDTO, Booking>();
        }
    }
}
