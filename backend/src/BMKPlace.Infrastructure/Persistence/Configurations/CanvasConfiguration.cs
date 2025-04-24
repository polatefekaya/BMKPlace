using System;
using BMKPlace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMKPlace.Infrastructure.Persistence.Configurations;

internal class CanvasConfiguration : IEntityTypeConfiguration<Canvas>
{
    public void Configure(EntityTypeBuilder<Canvas> builder)
    {
        // Set Primary Key
        builder.HasKey(c => c.Id);

        // Configure Id with value generation (assuming PostgreSQL SERIAL or IDENTITY)
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        // Configure Name property
        builder.Property(c => c.Name)
            .HasMaxLength(100) // Example max length
            .IsRequired();

        // Configure Width and Height (simple properties, EF Core handles defaults)
        builder.Property(c => c.Width)
            .IsRequired();

        builder.Property(c => c.Height)
            .IsRequired();

        // Configure DefaultCooldown
        builder.Property(c => c.DefaultCooldown)
            .IsRequired();

        // Configure IsActive
        builder.Property(c => c.IsActive)
            .IsRequired();

        // Configure Foreign Key for ColorPalette
        builder.Property(c => c.ColorPaletteId)
            .IsRequired();

        // Define the relationship explicitly (optional, but good practice)
        // Assumes a ColorPalette entity exists with a collection of Canvases (which it doesn't need)
        // We mainly care about the FK property Canvas.ColorPaletteId pointing to ColorPalette.Id
        builder.HasOne<ColorPalette>() // Specify the related entity type if navigation property is missing
               .WithMany()             // ColorPalette doesn't need a navigation property back to Canvas
               .HasForeignKey(c => c.ColorPaletteId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a palette if canvases use it

        // Add an index on Name for faster lookups (optional)
        builder.HasIndex(c => c.Name).IsUnique(); // Assuming canvas names should be unique

        // Add index for efficient lookup by active status
        builder.HasIndex(c => c.IsActive);
    }
}
