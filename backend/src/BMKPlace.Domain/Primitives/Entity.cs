using System;

namespace BMKPlace.Domain.Primitives;

public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : notnull
{
    public TId Id {get; protected init;}

    protected Entity(TId id){
        Id = id;
    }
    protected Entity(){}
    public override bool Equals(object? obj)
    {
        if (obj is null || GetType() != obj.GetType() || obj is not Entity<TId> other)
        {
            return false;
        }
        
        if(Id.Equals(default(TId)) || other.Id.Equals(default(TId))){
            return ReferenceEquals(this, other);
        }

        return Id.Equals(other.Id);
    }
    
    public override int GetHashCode()
    {
        const int HashMultiplier = 41;
        unchecked
        {
            return (GetType().GetHashCode() * HashMultiplier) + Id.GetHashCode();
        }
    }
    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right){
        if(left is null && right is null) return true;
        if(left is null || right is null) return false;

        return left.Equals(right);
    }
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right){
        return !(left == right);
    }
}
