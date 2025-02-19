

using Domain.Enum;

namespace Domain.Model
{
    public class CreateOrderResult
    {
        public string OrderId { get; set; }
        public string ApprovalUrl { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
