// FILE: src/BMKPlace.Infrastructure/Caching/RedisCanvasCache.cs
using BMKPlace.Application.Contracts.Abstractions.Infrastructure;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Application.Contracts.DTOs.Canvas;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using BMKPlace.Domain.Entities;
using BMKPlace.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using StackExchange.Redis; 
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BMKPlace.Infrastructure.Caching;

internal record CanvasMetadataDto(int CanvasId, int Width, int Height, int ColorPaletteId, DateTimeOffset StateTimestamp);

internal sealed class RedisCanvasCache : ICanvasCache
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IColorPaletteRepository _paletteRepository;
    private readonly ILogger<RedisCanvasCache> _logger;
    private const string MetaKeyPrefix = "canvas_meta:";
    private const string PixelsKeyPrefix = "canvas_pixels:";

    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public RedisCanvasCache(
        IConnectionMultiplexer redis, 
        IColorPaletteRepository paletteRepository,
        ILogger<RedisCanvasCache> logger)
    {
        _redis = redis;
        _paletteRepository = paletteRepository;
        _logger = logger;
    }

    private static RedisKey GetMetaCacheKey(int canvasId) => new RedisKey($"{MetaKeyPrefix}{canvasId}");
    private static RedisKey GetPixelsCacheKey(int canvasId) => new RedisKey($"{PixelsKeyPrefix}{canvasId}");

    public async Task<CanvasStateDto?> GetCachedCanvasStateAsync(int canvasId, CancellationToken cancellationToken = default)
    {
        RedisKey metaKey = GetMetaCacheKey(canvasId);
        RedisKey pixelsKey = GetPixelsCacheKey(canvasId);
        _logger.LogDebug("Attempting to get canvas state from cache. MetaKey: {MetaKey}, PixelsKey: {PixelsKey}", metaKey, pixelsKey);

        try
        {
            IDatabase db = _redis.GetDatabase();
            // Fetch both keys, potentially in a transaction for consistency, though separate reads are often fine.
            RedisValue metaJson = await db.StringGetAsync(metaKey);
            RedisValue pixelBytes = await db.StringGetAsync(pixelsKey);

            if (metaJson.HasValue && pixelBytes.HasValue)
            {
                _logger.LogInformation("Cache hit for canvas {CanvasId}", canvasId);
                CanvasMetadataDto? meta = JsonSerializer.Deserialize<CanvasMetadataDto>(metaJson!, _jsonOptions);
                if (meta is null)
                {
                     _logger.LogError("Failed to deserialize metadata for canvas {CanvasId}", canvasId);
                     return null;
                }

                return new CanvasStateDto(
                    meta.CanvasId,
                    meta.Width,
                    meta.Height,
                    meta.ColorPaletteId,
                    (byte[])pixelBytes!, // Cast RedisValue to byte[]
                    meta.StateTimestamp
                );
            }
            else
            {
                _logger.LogInformation("Cache miss for canvas {CanvasId}. Meta null: {IsMetaNull}, Pixels null: {IsPixelsNull}",
                    canvasId, !metaJson.HasValue, !pixelBytes.HasValue);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving canvas state from cache for canvas {CanvasId}", canvasId);
            return null; // Treat cache errors as a cache miss
        }
    }

    public async Task SetCachedCanvasStateAsync(CanvasStateDto canvasStateDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(canvasStateDto);
        RedisKey metaKey = GetMetaCacheKey(canvasStateDto.CanvasId);
        RedisKey pixelsKey = GetPixelsCacheKey(canvasStateDto.CanvasId);
        _logger.LogDebug("Attempting to set canvas state in cache. MetaKey: {MetaKey}, PixelsKey: {PixelsKey}", metaKey, pixelsKey);

        try
        {
            CanvasMetadataDto meta = new(
                canvasStateDto.CanvasId,
                canvasStateDto.Width,
                canvasStateDto.Height,
                canvasStateDto.ColorPaletteId,
                canvasStateDto.StateTimestamp);

            byte[] metaBytes = JsonSerializer.SerializeToUtf8Bytes(meta, _jsonOptions);
            byte[] pixelBytes = canvasStateDto.PixelData;

            IDatabase db = _redis.GetDatabase();
            // Use a transaction to set both keys atomically
            ITransaction tran = db.CreateTransaction();
#pragma warning disable CS8602 // Dereference of a possibly null reference. Transaction object is not null here.
            _ = tran.StringSetAsync(metaKey, metaBytes); // No expiry for now
            _ = tran.StringSetAsync(pixelsKey, pixelBytes); // No expiry for now
#pragma warning restore CS8602
            bool committed = await tran.ExecuteAsync();

            if (committed)
            {
                _logger.LogInformation("Successfully set canvas state in cache for canvas {CanvasId}", canvasStateDto.CanvasId);
            }
            else
            {
                 _logger.LogWarning("Failed to commit transaction while setting canvas state for canvas {CanvasId}", canvasStateDto.CanvasId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting canvas state in cache for canvas {CanvasId}", canvasStateDto.CanvasId);
        }
    }

    public async Task UpdatePixelInCacheAsync(PixelDto pixelDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pixelDto);
        RedisKey metaKey = GetMetaCacheKey(pixelDto.CanvasId);
        RedisKey pixelsKey = GetPixelsCacheKey(pixelDto.CanvasId);
         _logger.LogDebug("Attempting efficient pixel update in cache. PixelsKey: {PixelsKey} at ({X},{Y})", pixelsKey, pixelDto.X, pixelDto.Y);

        try
        {
            IDatabase db = _redis.GetDatabase();

            // 1. Get Metadata first to know Width and PaletteId
            RedisValue metaJson = await db.StringGetAsync(metaKey);
             if (!metaJson.HasValue)
             {
                  _logger.LogWarning("Cannot update pixel: Metadata not found in cache for canvas {CanvasId}", pixelDto.CanvasId);
                  return;
             }
             CanvasMetadataDto? meta = JsonSerializer.Deserialize<CanvasMetadataDto>(metaJson!, _jsonOptions);
              if (meta is null)
              {
                   _logger.LogError("Cannot update pixel: Failed to deserialize metadata for canvas {CanvasId}", pixelDto.CanvasId);
                   return;
              }

            // 2. Get Color Palette to find index (still requires DB lookup or better caching)
            ColorPalette? palette = await _paletteRepository.GetByIdAsync(meta.ColorPaletteId, cancellationToken);
            if (palette is null)
            {
                _logger.LogError("Cannot update pixel: ColorPalette {PaletteId} not found for canvas {CanvasId}.", meta.ColorPaletteId, pixelDto.CanvasId);
                return;
            }
            Color pixelColor = Color.Create(pixelDto.R, pixelDto.G, pixelDto.B);
            byte colorIndex = GetColorIndex(palette, pixelColor); // Reuse helper
            if (colorIndex == byte.MaxValue)
            {
                 _logger.LogWarning("Color {Color} from PixelDto not found in palette {PaletteId}. Cannot update cache.", pixelColor, palette.Id);
                 return;
            }

            // 3. Calculate offset
            long offset = (long)pixelDto.Y * meta.Width + pixelDto.X; // Use long for offset

            // 4. Prepare the single byte update
            byte[] colorIndexByte = [colorIndex];

            // 5. Use Redis transaction for atomic update of pixel byte and metadata timestamp
            CanvasMetadataDto updatedMeta = meta with { StateTimestamp = pixelDto.Timestamp };
            byte[] updatedMetaBytes = JsonSerializer.SerializeToUtf8Bytes(updatedMeta, _jsonOptions);

            ITransaction tran = db.CreateTransaction();
#pragma warning disable CS8602
            // Use StringSetRangeAsync which corresponds to SETRANGE command
            _ = tran.StringSetRangeAsync(pixelsKey, offset, colorIndexByte);
            _ = tran.StringSetAsync(metaKey, updatedMetaBytes); // Update metadata with new timestamp
#pragma warning restore CS8602
            bool committed = await tran.ExecuteAsync();

            if (committed)
            {
                 _logger.LogInformation("Successfully updated pixel at ({X},{Y}) and metadata timestamp using SETRANGE for canvas {CanvasId}", pixelDto.X, pixelDto.Y, pixelDto.CanvasId);
            }
            else
            {
                 _logger.LogWarning("Failed to commit transaction while updating pixel/metadata for canvas {CanvasId}", pixelDto.CanvasId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing efficient pixel update in cache for canvas {CanvasId} at ({X},{Y})", pixelDto.CanvasId, pixelDto.X, pixelDto.Y);
        }
    }

    private byte GetColorIndex(ColorPalette palette, Color color)
    {
        // Inefficient linear search - should be replaced with dictionary lookup
        // if palette data is cached or loaded more efficiently within this service.
        for (int i = 0; i < palette.AllowedColors.Count; i++)
        {
            if (palette.AllowedColors[i] == color) return (byte)i;
        }
        return byte.MaxValue; // Not found
    }

    public async Task ClearCacheAsync(int canvasId, CancellationToken cancellationToken = default)
    {
        RedisKey metaKey = GetMetaCacheKey(canvasId);
        RedisKey pixelsKey = GetPixelsCacheKey(canvasId);
        _logger.LogInformation("Attempting to clear cache for canvas {CanvasId}. Keys: {MetaKey}, {PixelsKey}", canvasId, metaKey, pixelsKey);
        try
        {
            IDatabase db = _redis.GetDatabase();
            // Delete both keys, potentially in a transaction
            ITransaction tran = db.CreateTransaction();
#pragma warning disable CS8602
            _ = tran.KeyDeleteAsync(metaKey);
            _ = tran.KeyDeleteAsync(pixelsKey);
#pragma warning restore CS8602
            bool committed = await tran.ExecuteAsync();

             if (committed)
            {
                _logger.LogInformation("Successfully cleared cache for canvas {CanvasId}", canvasId);
            }
            else
            {
                 _logger.LogWarning("Failed to commit transaction while clearing cache for canvas {CanvasId}", canvasId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache for canvas {CanvasId}", canvasId);
        }
    }
}
