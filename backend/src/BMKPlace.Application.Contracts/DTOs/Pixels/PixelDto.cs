namespace BMKPlace.Application.Contracts.DTOs.Pixels;

public record PixelDto(
    int CanvasId, 
    int X,
    int Y,
    int R, int G, int B,
    int UserId,
    DateTimeOffset Timestamp
);
