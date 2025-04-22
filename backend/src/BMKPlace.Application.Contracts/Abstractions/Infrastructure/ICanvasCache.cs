using System;
using BMKPlace.Application.Contracts.DTOs.Canvas;
using BMKPlace.Application.Contracts.DTOs.Pixels;

namespace BMKPlace.Application.Contracts.Abstractions.Infrastructure;

public interface ICanvasCache
{
    Task<CanvasStateDto?> GetCachedCanvasStateAsync(int canvasId, CancellationToken cancellationToken = default); 
    Task UpdatePixelInCacheAsync(PixelDto pixelDto, CancellationToken cancellationToken = default); 
    Task ClearCacheAsync(int canvasId, CancellationToken cancellationToken = default); 
    Task SetCachedCanvasStateAsync(CanvasStateDto canvasStateDto, CancellationToken cancellationToken = default); 
}
