using System;
using BMKPlace.Domain.Entities;
using BMKPlace.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMKPlace.Infrastructure.Persistence.Configurations;

internal class ColorPaletteConfiguration : IEntityTypeConfiguration<ColorPalette>
{
    public void Configure(EntityTypeBuilder<ColorPalette> builder)
    {
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Id)
            .ValueGeneratedOnAdd();

        builder.Property(cp => cp.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Configure the collection of Color Value Objects
        // This uses OwnsMany to map the collection to a separate dependent table.
        builder.OwnsMany<Color>("_allowedColors", // Use the private field name
            ownedBuilder =>
            {
                // Define the table name for the owned collection
                ownedBuilder.ToTable("AllowedPaletteColors");

                // Define the foreign key back to the ColorPalette table
                // EF Core generates "ColorPaletteId" by convention, but explicitly defining is safer.
                ownedBuilder.WithOwner().HasForeignKey("ColorPaletteId");

                // Define a composite primary key for this dependent table row
                // (ColorPaletteId + R + G + B ensures uniqueness of a color within a palette)
                ownedBuilder.HasKey("ColorPaletteId", nameof(Color.Red), nameof(Color.Green), nameof(Color.Blue));

                // Configure the properties of the owned Color Value Object
                // These map to columns in the AllowedPaletteColors table
                ownedBuilder.Property(c => c.Red).IsRequired();
                ownedBuilder.Property(c => c.Green).IsRequired();
                ownedBuilder.Property(c => c.Blue).IsRequired();

                // Optional: Add index on the FK for faster lookups of colors for a palette
                ownedBuilder.HasIndex("ColorPaletteId");
            });

        // Configure access to the collection through the private field
        builder.Navigation("_allowedColors")
            .HasField("_allowedColors")
            .UsePropertyAccessMode(PropertyAccessMode.Field); // Important for EF Core to use the field

        // Add index on Name (optional, if needed for lookups)
         builder.HasIndex(cp => cp.Name).IsUnique(); // Assuming palette names are unique
    }
}
