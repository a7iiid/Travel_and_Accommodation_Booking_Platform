

using Domain.Entities;
using Domain.Enum;

namespace Domain.Model
{
    public class BookingResult
    {
        public Guid Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public string PaymentStatus { get; set; }
        public string ApproveLink { get; set; }
        public string OrderId { get; set; }
        public Room Room { get; set; }



    }
}
