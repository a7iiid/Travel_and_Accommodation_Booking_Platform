﻿using Domain.Entities;


namespace Infrastructure.DB.Seeding
{
    public class CitySeeding
    {
        public static IEnumerable<City> SeedData()
        {
            return new List<City>
        {
            new()
            {
                Id = new Guid("f9e85d04-548c-4f98-afe9-2a8831c62a90"),
                Name = "New York",
                Country = "United States",
                PostOfficeCode = "NYC",
                CountryCode = "US"
            },
            new()
            {
                Id = new Guid("8d2aeb7a-7c67-4911-aa2c-d6a3b4dc7e9e"),
                Name = "London",
                Country = "United Kingdom",
                PostOfficeCode = "LDN",
                CountryCode = "UK"
            },
            new()
            {
                Id = new Guid("3c7e66f5-46a9-4b8d-8e90-85b5a9e2c2fd"),
                Name = "Tokyo",
                Country = "Japan",
                PostOfficeCode = "TKY",
                CountryCode = "JP"
            }
        };
        }
    }
}
