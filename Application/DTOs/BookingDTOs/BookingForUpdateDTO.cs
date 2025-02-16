

using Domain.Enum;

namespace Application.DTOs.BookingDTOs
{
    public record BookingForUpdateDTO
    {
        public Guid RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime BookingDate { get; set; }=DateTime.Now;
        public PaymentStatus PaymentStatus { get; set; }
    }
}
