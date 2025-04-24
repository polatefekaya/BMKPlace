using System;
using BMKPlace.Domain.ValueObjects;
using SharedKernel.Primitives;

namespace BMKPlace.Domain.Entities;

public sealed class ColorPalette : AggregateRoot<int>
{
    public string Name {get; private set;} = string.Empty;
    private readonly List<Color> _allowedColors = []; 
    public IReadOnlyList<Color> AllowedColors => _allowedColors;
    public const int RequiredColorCount = 16;

    private ColorPalette() : base() {}
    private ColorPalette(int id) : base(id){}

    public static ColorPalette Create(int id, string name, List<Color> allowedColors){
        if(id < 0){
            throw new ArgumentOutOfRangeException("Color Palette id must be positive");
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentNullException.ThrowIfNull(allowedColors, nameof(allowedColors));

        if(allowedColors.Count != RequiredColorCount){
            throw new ArgumentException($"Color Palette '{name}' must have exactly {RequiredColorCount} colors.");
        }
        ColorPalette palette = new(id) {Name = name};
        foreach (Color color in allowedColors) { palette._allowedColors.Add(color); }
        
        // palette.AddDomainEvent(new ColorPaletteCreatedEvent(palette.Id, palette.Name)); // Example event
        return palette;
    }

    public bool IsColorAllowed(Color color)
    {
         //ArgumentNullException.ThrowIfNull(color, nameof(color));
         return _allowedColors.Contains(color); // Equality check uses ValueObject implementation
    }
}
