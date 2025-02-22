

using Domain.Entities;
using Domain.Enum;

namespace Domain.Model
{
    public class Email
    {
        public string Name { get; set; }
        public Guid BookingId { get; set; }
        public double Amount { get; set; }
        public PaymentMethod  PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string ToEmail { get; set; }
        public Booking Booking { get; set; }
    }
}
