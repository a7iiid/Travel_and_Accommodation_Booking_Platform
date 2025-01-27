﻿

using Domain.Entities;

namespace Application.DTOs.BookingDTOs
{
    public  record BookingDTOForCreation
    {
        public Guid RoomId { get; set; }

        public Guid UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime BookingDate { get; set; }
        public double Price { get; set; }
        
       
    }
}
