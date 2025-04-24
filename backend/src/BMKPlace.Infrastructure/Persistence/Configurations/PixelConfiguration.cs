using System;
using BMKPlace.Domain.Entities;
using BMKPlace.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMKPlace.Infrastructure.Persistence.Configurations;

internal class PixelConfiguration : IEntityTypeConfiguration<Pixel>
{
    public void Configure(EntityTypeBuilder<Pixel> builder)
    {
        // Set Primary Key (long)
        builder.HasKey(p => p.Id);

        // Configure Id with value generation (assuming PostgreSQL BIGSERIAL or IDENTITY)
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        // Configure Foreign Key to Canvas
        builder.Property(p => p.CanvasId)
            .IsRequired();

        // Configure Foreign Key to User (using int as defined)
        builder.Property(p => p.UserId)
            .IsRequired();

        // Configure Timestamp
        builder.Property(p => p.Timestamp)
            .IsRequired();

        // Configure Coordinate Value Object as Owned Entity
        // Maps Coordinate properties (X, Y) to columns in the Pixels table
        builder.OwnsOne(p => p.Coordinate, coordBuilder =>
        {
            // Prefix column names to avoid clashes (e.g., "Coordinate_X")
            coordBuilder.Property(c => c.X).HasColumnName("CoordinateX").IsRequired();
            coordBuilder.Property(c => c.Y).HasColumnName("CoordinateY").IsRequired();
        });

        // Configure Color Value Object as Owned Entity
        // Maps Color properties (Red, Green, Blue) to columns in the Pixels table
        builder.OwnsOne(p => p.Color, colorBuilder =>
        {
            colorBuilder.Property(c => c.Red).HasColumnName("ColorRed").IsRequired();
            colorBuilder.Property(c => c.Green).HasColumnName("ColorGreen").IsRequired();
            colorBuilder.Property(c => c.Blue).HasColumnName("ColorBlue").IsRequired();
        });

        // --- Relationships ---

        // Relationship to Canvas (Many Pixels to One Canvas)
        builder.HasOne<Canvas>()
               .WithMany() // Canvas doesn't need a collection of Pixels
               .HasForeignKey(p => p.CanvasId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); // If Canvas is deleted, delete its pixels

        // Relationship to User (Many Pixels to One User)
        builder.HasOne<ApplicationUser>()
               .WithMany() // ApplicationUser doesn't need a collection of Pixels placed
               .HasForeignKey(p => p.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict); // Prevent user deletion if they have placed pixels? Or Cascade? Restrict is safer.

        // --- Indexes ---

        // Critical index for finding the latest pixel at a specific coordinate on a canvas
        // Used by GetLatestPixelForCoordinateAsync
        builder.HasIndex(p => new { p.CanvasId, p.Coordinate.X, p.Coordinate.Y, p.Timestamp })
               .HasDatabaseName("IX_Pixel_Canvas_Coordinate_Timestamp");
        // Note: Depending on query patterns, just indexing CanvasId, X, Y might be enough
        // if the query always orders by Timestamp descending and takes the first.
        // Let's start with this composite index. Consider analyzing query plans later.

         // Index for efficiently getting all latest pixels for a canvas (might help GetLatestPixelsForCanvasAsync)
         // This query pattern usually involves grouping by coordinates, so indexing CanvasId and Timestamp is key.
         builder.HasIndex(p => new { p.CanvasId, p.Timestamp })
                .HasDatabaseName("IX_Pixel_Canvas_Timestamp");

         // Index potentially useful for user-specific queries (e.g., "show my pixels")
         builder.HasIndex(p => new { p.UserId, p.Timestamp })
                .HasDatabaseName("IX_Pixel_User_Timestamp");
    }
}
