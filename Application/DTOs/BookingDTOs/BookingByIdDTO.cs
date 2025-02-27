﻿

using Application.DTOs.UserDTOs;
using Domain.Entities;
using Domain.Enum;

namespace Application.DTOs.BookingDTOs
{
    public class BookingByIdDTO
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime BookingDate { get; set; }
        public double Price { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public Payment? Payment { get; set; }
        public Review? Review { get; set; }
        public Room Room { get; set; }
        public UserWithOutBookingDTO User { get; set; }


    }
}
