

namespace Domain.Model
{
    public class Email
    {
        public string FirstName { get; set; }
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string ToEmail { get; set; }
    }
}
