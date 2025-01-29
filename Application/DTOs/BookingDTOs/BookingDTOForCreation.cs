

using Domain.Entities;
using Domain.Enum;

namespace Application.DTOs.BookingDTOs
{
    
        public record BookingDTOForCreation
        {
            public Guid RoomId { get; set; }
            public DateTime CheckInDate { get; set; }
            public DateTime CheckOutDate { get; set; }
            public PaymentMethod PaymentMethod { get; set; }
        }


    
}
