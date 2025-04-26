using System;
using BMKPlace.Domain.Entities;
using BMKPlace.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMKPlace.Infrastructure.Persistence.Configurations;

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {

        // Define the relationship to the School entity
        builder.HasOne<School>() // Navigation property on School not strictly needed
              .WithMany()      // School doesn't need a collection of Users
              .HasForeignKey(au => au.SchoolId) // Define the FK property
              .IsRequired() // Make it mandatory
              .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a school if users reference it

        // Add index on SchoolId for potentially looking up users by school
        builder.HasIndex(au => au.SchoolId);

    }
}
