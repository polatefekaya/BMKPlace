using System;
using BMKPlace.Domain.Exceptions;
using BMKPlace.Domain.ValueObjects;
using SharedKernel.Exceptions;
using SharedKernel.Primitives;

namespace BMKPlace.Domain.Entities;

public sealed class Canvas : AggregateRoot<int>
{
    public string Name {get; private set;} = string.Empty;
    public int Width {get; private set;}
    public int Height {get; private set;}
    public TimeSpan DefaultCooldown {get; private set;}
    public int ColorPaletteId {get; private set;}
    public bool IsActive {get; private set;}

    private Canvas() : base(){}

    private Canvas(int id) : base(id){}

    public static Canvas Create(int id, string name, int width, int height, TimeSpan defaultCooldown, int colorPaletteId, bool isActive = true){
        if (id <= 0) throw new ArgumentException("Canvas ID must be positive.", nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        if (width <= 0) throw new DomainValidationException("Canvas width must be positive.");
        if (height <= 0) throw new DomainValidationException("Canvas height must be positive.");
        if (defaultCooldown < TimeSpan.Zero) throw new DomainValidationException("Default cooldown cannot be negative.");
        if (colorPaletteId <= 0) throw new ArgumentException("Color Palette ID must be positive.", nameof(colorPaletteId)); // Changed check

        Canvas canvas = new(id)
        {
            Name = name,
            Width = width,
            Height = height,
            DefaultCooldown = defaultCooldown,
            ColorPaletteId = colorPaletteId,
            IsActive = isActive
        };
        // canvas.AddDomainEvent(new CanvasCreatedEvent(canvas.Id, ...)); // Example event
        return canvas;
    }

    public void UpdateSettings(string name, int width, int height, TimeSpan defaultCooldown, int colorPaletteId) // colorPaletteId is long
    {
         ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
         if (width <= 0) throw new DomainValidationException("Canvas width must be positive.");
         if (height <= 0) throw new DomainValidationException("Canvas height must be positive.");
         if (defaultCooldown < TimeSpan.Zero) throw new DomainValidationException("Default cooldown cannot be negative.");
         if (colorPaletteId <= 0) throw new ArgumentException("Color Palette ID must be positive.", nameof(colorPaletteId)); // Changed check

         bool changed = this.Name != name || this.Width != width || this.Height != height ||
                        this.DefaultCooldown != defaultCooldown || this.ColorPaletteId != colorPaletteId;

         if(changed)
         {
             this.Name = name;
             this.Width = width;
             this.Height = height;
             this.DefaultCooldown = defaultCooldown;
             this.ColorPaletteId = colorPaletteId;
             // AddDomainEvent(new CanvasSettingsUpdatedEvent(this.Id, ...)); // Example event
         }
    }
    
    public void Activate()
    {
        if(!this.IsActive) {
            this.IsActive = true; 
            // AddDomainEvent(new CanvasActivatedEvent(this.Id, ...));
        }
    }

    public void Deactivate()
    {
         if(this.IsActive) {
            this.IsActive = false; 
            // AddDomainEvent(new CanvasDeactivatedEvent(this.Id, ...));
         }
    }

    public bool IsCoordinateWithinBounds(Coordinate coordinate)
    {
         ArgumentNullException.ThrowIfNull(coordinate, nameof(coordinate));
         return coordinate.X >= 0 && coordinate.X < this.Width &&
                coordinate.Y >= 0 && coordinate.Y < this.Height;
    }

}
