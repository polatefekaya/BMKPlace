using System;
using BMKPlace.Domain.Entities;
using BMKPlace.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMKPlace.Infrastructure.Persistence.Configurations;

internal class CanvasUserContextConfiguration : IEntityTypeConfiguration<CanvasUserContext>
{
    public void Configure(EntityTypeBuilder<CanvasUserContext> builder)
    {
        // Set Primary Key (long)
        builder.HasKey(ctx => ctx.Id);

        // Configure Id with value generation
        builder.Property(ctx => ctx.Id)
            .ValueGeneratedOnAdd();

        // Configure Foreign Keys
        builder.Property(ctx => ctx.CanvasId)
            .IsRequired();

        builder.Property(ctx => ctx.UserId) // int FK
            .IsRequired();

        // Configure Enum property (stored as integer by default)
        builder.Property(ctx => ctx.Role)
            .IsRequired();

        // Configure Nullable properties
        builder.Property(ctx => ctx.LastPixelPlacementTime)
            .IsRequired(false); // Explicitly marking as optional

        builder.Property(ctx => ctx.OverrideCooldown)
            .IsRequired(false);

        // Configure Boolean property
        builder.Property(ctx => ctx.IsBanned)
            .IsRequired();

        // --- Relationships ---

        // Relationship to Canvas
        builder.HasOne<Canvas>()
               .WithMany() // Canvas doesn't need a collection of contexts
               .HasForeignKey(ctx => ctx.CanvasId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); // If Canvas is deleted, delete associated contexts

        // Relationship to User
        builder.HasOne<ApplicationUser>()
               .WithMany() // User doesn't need a collection of canvas contexts
               .HasForeignKey(ctx => ctx.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); // If User is deleted, delete their contexts

        // --- Constraints and Indexes ---

        // Ensure a user can only have one context entry per canvas
        builder.HasIndex(ctx => new { ctx.CanvasId, ctx.UserId })
               .IsUnique()
               .HasDatabaseName("IX_CanvasUserContext_CanvasId_UserId_Unique");

        // Index for potentially finding contexts by user quickly
        builder.HasIndex(ctx => ctx.UserId)
               .HasDatabaseName("IX_CanvasUserContext_UserId");

         // Index for finding banned users on a canvas
         builder.HasIndex(ctx => new { ctx.CanvasId, ctx.IsBanned })
                .HasDatabaseName("IX_CanvasUserContext_Canvas_Banned");
    }
}
