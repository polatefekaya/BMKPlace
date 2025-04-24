using System;
using BMKPlace.Domain.Enums;
using BMKPlace.Domain.Exceptions;
using SharedKernel.Exceptions;
using SharedKernel.Primitives;

namespace BMKPlace.Domain.Entities;

public sealed class CanvasUserContext : AggregateRoot<long>
{
    public int CanvasId {get; private set;}
    public int UserId {get; private set;}
    public CanvasUserRole Role {get; private set;}
    public DateTimeOffset? LastPixelPlacementTime {get; private set;}
    public TimeSpan? OverrideCooldown {get; private set;}
    public bool IsBanned {get; private set;}

    private CanvasUserContext() : base(){}

    private CanvasUserContext(long id) : base(id){}

    public static CanvasUserContext Create(long id, int canvasId, int userId, CanvasUserRole initialRole = CanvasUserRole.Participant){
        if (id <= 0) throw new ArgumentException("Context ID must be positive.", nameof(id));
        if (canvasId <= 0) throw new ArgumentException("Canvas ID must be positive.", nameof(canvasId));
        if (userId <= 0) throw new ArgumentException("User ID must be positive.", nameof(userId));

        CanvasUserContext context = new(id)
        {
            CanvasId = canvasId,
            UserId = userId,
            Role = initialRole,
            IsBanned = false,
            LastPixelPlacementTime = null,
            OverrideCooldown = null
        };
        // context.AddDomainEvent(new CanvasUserContextCreatedEvent(...)); 
        return context;
    }

    public void RecordPixelPlacement(DateTimeOffset placementTime)
    {
        if(this.IsBanned) throw new UserBannedException(this.UserId, this.CanvasId);
        this.LastPixelPlacementTime = placementTime;
    }

    public DateTimeOffset? GetCooldownEndTime(TimeSpan canvasDefaultCooldown)
    {
        if (this.LastPixelPlacementTime is null) return null;
        TimeSpan applicableCooldown = this.OverrideCooldown ?? canvasDefaultCooldown;
        if (applicableCooldown < TimeSpan.Zero) applicableCooldown = TimeSpan.Zero;
        return this.LastPixelPlacementTime.Value + applicableCooldown;
    }

    public bool IsOnCooldown(DateTimeOffset currentTime, TimeSpan canvasDefaultCooldown)
    {
        DateTimeOffset? cooldownEndTime = GetCooldownEndTime(canvasDefaultCooldown);
        return cooldownEndTime.HasValue && currentTime < cooldownEndTime.Value;
    }

    public void AssignRole(CanvasUserRole newRole)
    {
        if(this.Role != newRole) {
            this.Role = newRole;
            // AddDomainEvent(new CanvasUserRoleChangedEvent(this.Id, this.UserId, this.CanvasId, newRole));
        }
    }

    public void BanUser()
    {
         if(!this.IsBanned) { 
            this.IsBanned = true;
             // AddDomainEvent(new CanvasUserBannedEvent(this.Id, this.UserId, this.CanvasId));
        }
    }

    public void UnbanUser()
    {
         if(this.IsBanned)
         {
              this.IsBanned = false;
              // AddDomainEvent(new CanvasUserUnbannedEvent(this.Id, this.UserId, this.CanvasId));
         }
    }

    public void SetOverrideCooldown(TimeSpan? cooldown)
    {
        if(cooldown.HasValue && cooldown.Value < TimeSpan.Zero)
            throw new DomainValidationException("Override cooldown cannot be negative."); // Or specific exception?

        if(this.OverrideCooldown != cooldown)
        {
            this.OverrideCooldown = cooldown;
            // AddDomainEvent(new CanvasUserCooldownOverriddenEvent(this.Id, ...));
        }
    }


}
