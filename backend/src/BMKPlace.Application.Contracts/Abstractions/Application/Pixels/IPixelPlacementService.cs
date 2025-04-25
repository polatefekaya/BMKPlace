using System;
using BMKPlace.Application.Contracts.DTOs.Pixels;

namespace BMKPlace.Application.Contracts.Abstractions.Application.Pixels;

public interface IPixelPlacementService
{
    public Task<PixelDto> ExecutePlacementAsync(
        int userId,
        int canvasId,
        int x, int y, // Coordinate primitives
        int r, int g, int b, // Color primitives
        string commandTypeName, // For logging context
        CancellationToken cancellationToken);
}
