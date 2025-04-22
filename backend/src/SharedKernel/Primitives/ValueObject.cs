using System;

namespace SharedKernel.Primitives;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract IEnumerable<object> GetAtomicValues();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType() || obj is not ValueObject other){
            return false;
        }
        
        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }
    public override int GetHashCode()
    {
        unchecked{
            int hash = 17;
            const int hashMultiplier = 23;

            foreach(object? value in GetAtomicValues()){
                hash = hash * hashMultiplier + (value?.GetHashCode() ?? 0);
            }

            return hash;
        }
    }
    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right){
        if(left is null && right is null) return true;
        if(left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right){
        return !(left == right);
    }
}
