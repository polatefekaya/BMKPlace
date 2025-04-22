using System;
using BMKPlace.Domain.Exceptions;
using BMKPlace.Domain.ValueObjects;
using SharedKernel.Primitives;

namespace BMKPlace.Domain.Entities;

public sealed class Pixel : AggregateRoot<long>
{
    public int CanvasId {get; private set;}
    public Coordinate Coordinate {get; private set;} = null!;
    public Color Color {get; private set;} = null!;
    public long UserId {get; private set;}
    public DateTimeOffset Timestamp {get; private set;}

    private Pixel() : base(){}

    private Pixel(long id) : base(id){}

    public static Pixel Create(long id, int canvasId, Coordinate coordinate, Color color, long userId, DateTimeOffset timestamp){
        if (id <= 0) throw new ArgumentException("Pixel ID must be positive.", nameof(id));
        if (canvasId <= 0) throw new ArgumentException("Canvas ID must be positive.", nameof(canvasId));
        ArgumentNullException.ThrowIfNull(coordinate, nameof(coordinate));
        ArgumentNullException.ThrowIfNull(color, nameof(color));
        if (userId <= 0) throw new ArgumentException("User ID must be positive.", nameof(userId));
        if (timestamp == default) throw new ArgumentException("Timestamp must be provided.", nameof(timestamp));

        Pixel pixel = new(id)
        {
            CanvasId = canvasId,
            Coordinate = coordinate,
            Color = color,
            UserId = userId,
            Timestamp = timestamp
        };

        //pixel.AddDomainEvent(new PixelPlacedEvent(pixel.Id, pixel.CanvasId, pixel.Coordinate, pixel.Color, pixel.UserId, pixel.Timestamp));
        return pixel;
    }
}
