using System;
using BMKPlace.Domain.Entities;
using BMKPlace.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BMKPlace.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    // DbSets for our Domain Aggregates/Entities
    public DbSet<Canvas> Canvases => Set<Canvas>();
    public DbSet<Pixel> Pixels => Set<Pixel>();
    public DbSet<ColorPalette> ColorPalettes => Set<ColorPalette>();
    public DbSet<CanvasUserContext> CanvasUserContexts => Set<CanvasUserContext>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // builder.HasDefaultSchema("bmk");
        //
        // --- Example: Configure Identity table names if needed ---
        // builder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "Users"); });
        // builder.Entity<IdentityRole<int>>(entity => { entity.ToTable(name: "Roles"); });
        // builder.Entity<IdentityUserRole<int>>(entity => { entity.ToTable("UserRoles"); });
        // builder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable("UserClaims"); });
        // builder.Entity<IdentityUserLogin<int>>(entity => { entity.ToTable("UserLogins"); });
        // builder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable("RoleClaims"); });
        // builder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable("UserTokens"); });

        // Additional model configurations will go in separate IEntityTypeConfiguration classes.
    }

    // Optional: Override SaveChangesAsync later if needed for domain event dispatching
    // public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    // {
    //     // Dispatch Domain Events logic here potentially
    //     return await base.SaveChangesAsync(cancellationToken);
    // }
}
