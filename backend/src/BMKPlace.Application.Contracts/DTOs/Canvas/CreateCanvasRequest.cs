using System.ComponentModel.DataAnnotations;

namespace BMKPlace.Application.Contracts.DTOs.Canvas;

public sealed record CreateCanvasRequest(
    [Required]
    [StringLength(100, MinimumLength = 3)]
    string Name,

    [Range(10, 2000)] // Example reasonable bounds
    int Width,

    [Range(10, 2000)]
    int Height,

    [Range(0, 86400)] // Cooldown in seconds (0 to 1 day)
    int DefaultCooldownSeconds,

    [Required]
    int ColorPaletteId,

    [Required]
    int SchoolId
);
