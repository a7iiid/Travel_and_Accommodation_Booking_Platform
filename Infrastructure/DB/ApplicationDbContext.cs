﻿using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.DB
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RoomAmenity> RoomAmenities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new BookingConfiguration());
            //modelBuilder.ApplyConfiguration(new CityConfiguration());
            //modelBuilder.ApplyConfiguration(new HotelConfiguration());
            //modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            //modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            //modelBuilder.ApplyConfiguration(new RoomConfiguration());
            //modelBuilder.ApplyConfiguration(new RoomTypeConfiguration());
            //modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder.SeedTables();
            base.OnModelCreating(modelBuilder);
        }


    }
}
