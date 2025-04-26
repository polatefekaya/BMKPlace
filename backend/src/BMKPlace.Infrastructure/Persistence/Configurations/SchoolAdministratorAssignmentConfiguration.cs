using System;
using BMKPlace.Domain.Entities;
using BMKPlace.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMKPlace.Infrastructure.Persistence.Configurations;

internal class SchoolAdministratorAssignmentConfiguration : IEntityTypeConfiguration<SchoolAdministratorAssignment>
{
    public void Configure(EntityTypeBuilder<SchoolAdministratorAssignment> builder)
    {
        builder.HasKey(saa => new { saa.UserId, saa.SchoolId });

        builder.Property(saa => saa.AssignedOnUtc).IsRequired();

        builder.HasOne<ApplicationUser>() 
               .WithMany()                
               .HasForeignKey(saa => saa.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne<School>()          
               .WithMany()                
               .HasForeignKey(saa => saa.SchoolId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); 
    }
}
