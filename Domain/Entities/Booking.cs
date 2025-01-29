namespace Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public double Price { get; set; }
        public Payment? Payment { get; set; }
        public Review? Review { get; set; } // Keep this relationship
        public Room Room { get; set; }
        public User User { get; set; }
    }
}