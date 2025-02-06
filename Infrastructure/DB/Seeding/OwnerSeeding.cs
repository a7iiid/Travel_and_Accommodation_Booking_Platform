
using Domain.Entities;

namespace Infrastructure.DB.Seeding
{
    public class OwnerSeeding
    {
        public static List<Owner> SeedData()
        {
            return new List<Owner>
        {
            new()
            {
                Id = new Guid("a1d1aa11-12e7-4e0f-8425-67c1c1e62c2d"),
                FirstName = "Ahmad",
                LastName = "Nazzal",
                Email = "ahmad@gmail.com",
                PhoneNumber = "0568759755",
            },
            new()
            {
                Id = new Guid("77b2c30b-65d0-4ea7-8a5e-71e7c294f117"),
                FirstName = "Omar",
                LastName = "Nazzal",
                Email = "omar@gmail.com",
                PhoneNumber = "0597856131",
            }
        };
        }
    }
}
