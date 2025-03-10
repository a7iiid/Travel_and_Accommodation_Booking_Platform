﻿using Domain.Entities;
using Domain.Enum;


namespace Infrastructure.DB.Seeding
{
    public class RoomTypeSeeding
    {
        public static IEnumerable<RoomType> SeedData()
        {
            return new List<RoomType>
        {
            new()
            {
                Id = new Guid("5a5de3b8-3ed8-4f0a-bda9-cf73225a64a1"),
                HotelId = new Guid("98c2c9fe-1a1c-4eaa-a7f5-b9d19b246c27"),
                Category = RoomCategory.Deluxe,
                PricePerNight = 100.0f
            },
            new()
            {
                Id = new Guid("d67ddbe4-1f1a-4d85-bcc1-ec3a475ecb68"),
                HotelId = new Guid("bfa4173d-7893-48b9-a497-5f4c7fb2492b"),
                Category = RoomCategory.Economy,
                PricePerNight = 150.0f
            },
            new()
            {
                Id = new Guid("4b4c0ea5-0b9a-4a20-8ad9-77441fb912d2"),
                HotelId = new Guid("9461e08b-92d3-45da-b6b3-efc0cfcc4a3a"),
                Category = RoomCategory.Suite,
                PricePerNight = 200.0f
            }
        };
        }
    }
}
