using System.ComponentModel.DataAnnotations;

namespace BMKPlace.Application.Contracts.DTOs.Canvas;

public sealed record UpdateCanvasSettingsRequest(
    [Required]
    [StringLength(100, MinimumLength = 3)]
    string Name,

    [Range(10, 2000)]
    int Width,

    [Range(10, 2000)]
    int Height,

    [Range(0, 86400)]
    int DefaultCooldownSeconds,

    [Required]
    int ColorPaletteId
);
