

using Domain.Enum;

namespace Domain.Entities
{
    public class Room
    {
        public Guid Id { get; set; }
        public Guid RoomTypeId { get; set; }
        public int AdultsCapacity { get; set; }
        public int ChildrenCapacity { get; set; }
        public float Rating { get; set; }
        public RoomType RoomType { get; set; }
    }
}

