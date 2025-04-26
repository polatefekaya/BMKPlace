using System;
using BMKPlace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMKPlace.Infrastructure.Persistence.Configurations;

internal class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd(); // Or ValueGeneratedNever if IDs are assigned externally

        builder.Property(s => s.Name)
            .HasMaxLength(200) // Example length
            .IsRequired();

        builder.HasIndex(s => s.Name).IsUnique(); // School names should probably be unique
    }
}
