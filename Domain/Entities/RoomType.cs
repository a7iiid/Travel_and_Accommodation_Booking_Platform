using Domain.Enum;


namespace Domain.Entities
{
    public class RoomType
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public RoomCategory Category { get; set; }
        public float PricePerNight { get; set; }
        public IList<RoomAmenity> Amenities { get; set; } = new List<RoomAmenity>();
        public IList<Room> Rooms { get; set; } = new List<Room>();
    }

}
