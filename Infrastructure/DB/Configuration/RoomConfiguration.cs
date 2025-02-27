﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Common.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder
            .HasOne<RoomType>()
            .WithMany()
            .HasForeignKey(room => room.RoomTypeId)
            
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .Property(room => room.Rating)
            .IsRequired()
            .HasDefaultValue(0.0F);

        

      

        

        builder.ToTable(room =>
            room
            .HasCheckConstraint
            ("CK_Review_RatingRange", "[Rating] >= 0 AND [Rating] <= 5"));
    }
}