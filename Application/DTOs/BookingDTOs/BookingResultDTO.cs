

using Application.DTOs.RoomDTOs;
using Domain.Entities;
using Domain.Enum;

namespace Application.DTOs.BookingDTOs
{
    public class BookingResultDTO
    {
        public Guid Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public string PaymentStatus { get; set; }
        public string ApproveLink { get; set; }
        public RoomDTO Room { get; set; }



    }
}
