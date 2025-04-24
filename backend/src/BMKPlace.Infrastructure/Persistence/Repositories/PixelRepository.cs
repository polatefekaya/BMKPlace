using System;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Domain.Entities;
using BMKPlace.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BMKPlace.Infrastructure.Persistence.Repositories;

internal sealed class PixelRepository : IPixelRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PixelRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(Pixel pixel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pixel);
        await _dbContext.Pixels.AddAsync(pixel, cancellationToken);
        // SaveChangesAsync called by UnitOfWork
    }

    public async Task<Pixel?> GetLatestPixelForCoordinateAsync(int canvasId, Coordinate coordinate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(coordinate);

        // Query based on CanvasId and the owned Coordinate properties (X, Y).
        // Order by Timestamp descending to get the latest one first.
        return await _dbContext.Pixels
            .Where(p => p.CanvasId == canvasId &&
                        p.Coordinate.X == coordinate.X &&
                        p.Coordinate.Y == coordinate.Y)
            .OrderByDescending(p => p.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
        // AsNoTracking() could be used here if the pixel isn't intended for modification.
    }

    public async Task<IEnumerable<Pixel>> GetLatestPixelsForCanvasAsync(int canvasId, CancellationToken cancellationToken = default)
    {
        // Step 1: Define the subquery to get the max timestamp per coordinate for the canvas
        // Using System.Linq.IQueryable<dynamic> or an explicitly defined helper class/record if preferred
        var maxTimestampsPerCoordinate = _dbContext.Pixels
            .Where(p_inner => p_inner.CanvasId == canvasId)
            .GroupBy(p_inner => new { p_inner.Coordinate.X, p_inner.Coordinate.Y }) // Group by coordinate
            .Select(g => new // Select coordinate and the max timestamp
            {
                g.Key.X,
                g.Key.Y,
                MaxTimestamp = g.Max(p_inner => p_inner.Timestamp)
            });

        // Step 2: Join the main Pixel table with the result of the subquery
        var latestPixelsQuery = _dbContext.Pixels
            .Where(p => p.CanvasId == canvasId)
            .Join(
                inner: maxTimestampsPerCoordinate, // Join with the max timestamps query
                outerKeySelector: p => new { p.Coordinate.X, p.Coordinate.Y }, // Key from the outer Pixel table (coordinate)
                innerKeySelector: mt => new { mt.X, mt.Y }, // Key from the inner max timestamps query (coordinate)
                resultSelector: (pixel, maxInfo) => new { Pixel = pixel, MaxTimestamp = maxInfo.MaxTimestamp } // Project into intermediate type
            )
            // Step 3: Filter the joined results to only include pixels whose timestamp matches the max timestamp for their coordinate
            .Where(joined => joined.Pixel.Timestamp == joined.MaxTimestamp)
            // Step 4: Select the final Pixel entity
            .Select(joined => joined.Pixel);


        // Execute the query
        return await latestPixelsQuery
                     // .AsNoTracking() // Consider uncommenting if the results are read-only
                     .ToListAsync(cancellationToken);

    }
}
