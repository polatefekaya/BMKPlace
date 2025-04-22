using System;
using BMKPlace.Domain.Entities;
using BMKPlace.Domain.ValueObjects;

namespace BMKPlace.Application.Contracts.Abstractions.Persistence;

public interface IPixelRepository
{
    Task AddAsync(Pixel pixel, CancellationToken cancellationToken = default); // Pixel uses long ID
    Task<Pixel?> GetLatestPixelForCoordinateAsync(int canvasId, Coordinate coordinate, CancellationToken cancellationToken = default); // canvasId is int
    Task<IEnumerable<Pixel>> GetLatestPixelsForCanvasAsync(int canvasId, CancellationToken cancellationToken = default); // canvasId is int
}
