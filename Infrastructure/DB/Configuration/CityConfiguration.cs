using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Common.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder
            .Property(city => city.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(owner => owner.CountryCode)
            .IsRequired()
            .HasMaxLength(3);

        builder
            .Property(hotel => hotel.PostOfficeCode)
            .IsRequired()
            .HasMaxLength(10);

        
    }
}