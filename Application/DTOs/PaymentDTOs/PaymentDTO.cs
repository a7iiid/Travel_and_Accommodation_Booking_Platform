

using Domain.Enum;

namespace Application.DTOs.PaymentDTOs
{
    public class PaymentDTO
    {
        public Guid BookingId { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public double Amount { get; set; }
    }
}
