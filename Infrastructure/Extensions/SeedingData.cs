using Domain.Entities;
using Infrastructure.DB.Seeding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class SeedingData
    {
        public static void SeedTables(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(CitySeeding.SeedData());
            modelBuilder.Entity<Hotel>().HasData(HotelSeeding.SeedData());
            modelBuilder.Entity<RoomType>().HasData(RoomTypeSeeding.SeedData());
            modelBuilder.Entity<Room>().HasData(RoomSeeding.SeedData());
            modelBuilder.Entity<User>().HasData(UserSeeding.SeedData());
            modelBuilder.Entity<Owner>().HasData(OwnerSeeding.SeedData());
            modelBuilder.Entity<Booking>().HasData(BookingSeeding.SeedData());
            modelBuilder.Entity<Payment>().HasData(PaymentSeeding.SeedData());
            modelBuilder.Entity<Review>().HasData(ReviewSeeding.SeedData());
        }
    }
}
