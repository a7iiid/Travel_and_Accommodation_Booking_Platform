using Domain.Entities;

namespace Infrastructure.DB.Seeding
{

    public class RoomSeeding
    {
        public static IEnumerable<Room> SeedData()
        {
            return new List<Room>
        {
            new()
            {
                Id = new Guid("a98b8a9d-4c5a-4a90-a2d2-5f1441b93db6"),
                RoomTypeId = new Guid("5a5de3b8-3ed8-4f0a-bda9-cf73225a64a1"),
                Capacity = 2,
                Rating = 4.5f,
                
            },
           
        };
        }
    }
}

