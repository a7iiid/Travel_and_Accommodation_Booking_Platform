

namespace Domain.Model
{
    public class Email
    {
        public string FirstName { get; set; }
        public Guid BookingId { get; set; }
        public double Amount { get; set; }
        public string ToEmail { get; set; }
    }
}
