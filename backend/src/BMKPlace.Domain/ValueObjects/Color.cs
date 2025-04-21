using System;
using BMKPlace.Domain.Primitives;

namespace BMKPlace.Domain.ValueObjects;

public sealed class Color : ValueObject
{
    public int Red {get;}
    public int Green {get;}
    public int Blue {get;}

    private Color(int r, int g, int b){
        if(r < byte.MinValue || g < byte.MinValue || b < byte.MinValue){
            //less than 0 throw
            throw new ArgumentOutOfRangeException($"({r},{g},{b}) is out of range (some of them less than {byte.MinValue})");
        }
        if(r > byte.MaxValue || g > byte.MaxValue || b > byte.MaxValue){
            //greater than 255 throw
            throw new ArgumentOutOfRangeException($"({r},{g},{b}) is out of range (some of them greater than {byte.MaxValue})");
        }

        Red = r;
        Green = g;
        Blue = b;

    }
    private Color(){}

    public static Color Create(int r, int g, int b){
        return new Color(r, g, b);
    }
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Red;
        yield return Green;
        yield return Blue;
    }

    public override string ToString() => $"rgb({Red},{Green},{Blue})";
}
