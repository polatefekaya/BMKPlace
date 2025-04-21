using System;
using BMKPlace.Domain.Primitives;

namespace BMKPlace.Domain.ValueObjects;

public sealed class Coordinate : ValueObject
{
    public int X {get;}
    public int Y {get;}

    private Coordinate(int x, int y){
        X = x;
        Y = y;
    }
    private Coordinate() {}
    public static Coordinate Create(int x, int y){
        return new Coordinate(x, y);
    }
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return X;
        yield return Y;
    }

    public override string ToString() => $"({X},{Y})";
}
