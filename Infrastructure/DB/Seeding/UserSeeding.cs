using Domain.Entities;

namespace Infrastructure.DB.Seeding
{
    public class UserSeeding
    {
        public static IEnumerable<User> SeedData()
        {
            return new List<User>
            {
                new()
                {
                    Id = new Guid("c6c45f7c-2dfe-4c1e-9a9b-8b173c71b32c"), 
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "1234567890",
                    PasswordHash = "hashedpassword1", 
                    Salt = "salt1",
                    isAdmin = false,
                    Bookings = new List<Booking>() 
                },
                new()
                {
                    Id = new Guid("aaf21a7d-8fc3-4c9f-8a8e-1eeec8dcd462"), 
                    FirstName = "Alice",
                    LastName = "Smith",
                    Email = "alice.smith@example.com",
                    PhoneNumber = "0987654321",
                    PasswordHash = "hashedpassword2",
                    Salt = "salt2",
                    isAdmin = false,
                    Bookings = new List<Booking>()
                },
                new()
                {
                    Id = new Guid("f44c3eb4-2c8a-4a77-a31b-04c4619aa15a"),
                    FirstName = "Robert",
                    LastName = "Johnson",
                    Email = "a@email.com",
                    PhoneNumber = "A@a123456",
                    PasswordHash = "hashedpassword3",
                    Salt = "salt3",
                    isAdmin = true,
                    Bookings = new List<Booking>()
                }
            };
        }
    }
}
