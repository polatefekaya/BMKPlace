using System;
using BMKPlace.Application.Contracts.Abstractions.Infrastructure;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Application.Contracts.DTOs.Canvas;
using BMKPlace.Domain.Entities;
using BMKPlace.Domain.ValueObjects;
using Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.PixelCanvas.Queries.GetCanvasState;

public sealed class GetCanvasStateHandler : IQueryHandler<GetCanvasStateQuery, CanvasStateDto>
{
    private readonly ICanvasCache _canvasCache;
    private readonly ICanvasRepository _canvasRepository;
    private readonly IPixelRepository _pixelRepository;
    private readonly IColorPaletteRepository _colorPaletteRepository;
    private readonly ILogger<GetCanvasStateHandler> _logger;
    private readonly IDateTimeService _dateTimeService; // To timestamp the generated state

    // Color representing the default background before any pixel is placed
    private static readonly Color DefaultBackgroundColor = Color.Create(255, 255, 255); // White, assumes Color.Create is available

    public GetCanvasStateHandler(
        ICanvasCache canvasCache,
        ICanvasRepository canvasRepository,
        IPixelRepository pixelRepository,
        IColorPaletteRepository colorPaletteRepository,
        ILogger<GetCanvasStateHandler> logger,
        IDateTimeService dateTimeService)
    {
        _canvasCache = canvasCache;
        _canvasRepository = canvasRepository;
        _pixelRepository = pixelRepository;
        _colorPaletteRepository = colorPaletteRepository;
        _logger = logger;
        _dateTimeService = dateTimeService;
    }

    public async ValueTask<CanvasStateDto> Handle(GetCanvasStateQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCanvasStateQuery for CanvasId {CanvasId}, IgnoreCache={IgnoreCache}", query.CanvasId, query.IgnoreCache);

        var cacheResult = await CheckCache(query, cancellationToken);
        if(cacheResult is not null) return cacheResult;

        _logger.LogDebug("Fetching canvas, palette, and latest pixels for CanvasId {CanvasId}", query.CanvasId);

        Canvas canvas = await _canvasRepository.GetByIdAsync(query.CanvasId, cancellationToken) 
            ?? throw new NotFoundException(nameof(Canvas), query.CanvasId);

        IEnumerable<Pixel> latestPixels = await _pixelRepository.GetLatestPixelsForCanvasAsync(query.CanvasId, cancellationToken);

        ColorPalette palette = await _colorPaletteRepository.GetByIdAsync(canvas.ColorPaletteId, cancellationToken)
            ?? throw new NotFoundException(nameof(ColorPalette), canvas.ColorPaletteId); // Data integrity issue

        _logger.LogDebug("Fetched {PixelCount} latest pixel events for CanvasId {CanvasId}", latestPixels.Count(), query.CanvasId);

        _logger.LogDebug("Building indexed pixel data for {Width}x{Height} canvas", canvas.Width, canvas.Height);

        var colorToIndexMap = palette.AllowedColors
                                    .Select((c, i) => new { Color = c, Index = (byte)i })
                                    .ToDictionary(x => x.Color, x => x.Index); // Uses Color VO equality

        if (!colorToIndexMap.TryGetValue(DefaultBackgroundColor, out byte defaultColorIndex))
        {
            // Fallback or error if white isn't in the palette? Use index 0? Log warning.
            _logger.LogWarning("Default background color {Color} not found in Palette {PaletteId}. Using index 0 as default.", DefaultBackgroundColor, palette.Id);
            defaultColorIndex = 0; 
        }

        // Initialize pixel data buffer with the default background color index.
        int canvasSize = canvas.Width * canvas.Height;
        byte[] indexedPixelData = new byte[canvasSize];
        Array.Fill(indexedPixelData, defaultColorIndex); // Pre-fill with background

        
        foreach (Pixel pixel in latestPixels)
        {
            int index = pixel.Coordinate.Y * canvas.Width + pixel.Coordinate.X;

            if (index >= 0 && index < canvasSize)
            {
                if (colorToIndexMap.TryGetValue(pixel.Color, out byte colorIndex))
                {
                    indexedPixelData[index] = colorIndex;
                }
                else
                {
                    // This indicates a pixel event with a color not in the canvas's current palette - data integrity issue?
                    _logger.LogWarning("Pixel {PixelId} at ({X},{Y}) on Canvas {CanvasId} has color {Color} which is not in the current Palette {PaletteId}. Using default index {DefaultIndex}.",
                        pixel.Id, pixel.Coordinate.X, pixel.Coordinate.Y, canvas.Id, pixel.Color, palette.Id, defaultColorIndex);
      
                    indexedPixelData[index] = defaultColorIndex;
                }
            }
            else
            {
                _logger.LogWarning("Pixel {PixelId} coordinate ({X},{Y}) is out of calculated bounds for Canvas {CanvasId} ({Width}x{Height}). Skipping.",
                    pixel.Id, pixel.Coordinate.X, pixel.Coordinate.Y, canvas.Id, canvas.Width, canvas.Height);
            }
        }
        _logger.LogDebug("Finished building indexed pixel data.");


        // 4. --- Create DTO & Update Cache ---
        var now = _dateTimeService.UtcNow;
        CanvasStateDto canvasStateDto = new CanvasStateDto(
            CanvasId: canvas.Id,
            Width: canvas.Width,
            Height: canvas.Height,
            ColorPaletteId: canvas.ColorPaletteId,
            PixelData: indexedPixelData,
            StateTimestamp: now
        );

        _logger.LogDebug("Attempting to set newly built canvas state in cache for CanvasId {CanvasId}", query.CanvasId);
        try
        {
            await _canvasCache.SetCachedCanvasStateAsync(canvasStateDto, cancellationToken);
            _logger.LogInformation("Successfully updated cache for CanvasId {CanvasId}", query.CanvasId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update cache after building canvas state for CanvasId {CanvasId}. Returning state anyway.", query.CanvasId);
            // Log failure but return the state regardless, cache will be stale.
        }

        return canvasStateDto;
    }

    private async Task<CanvasStateDto?> CheckCache(GetCanvasStateQuery query, CancellationToken cancellationToken){
        if (!query.IgnoreCache) {
            _logger.LogDebug("Attempting to retrieve canvas state from cache for CanvasId {CanvasId}.", query.CanvasId);
            CanvasStateDto? cachedState = await _canvasCache.GetCachedCanvasStateAsync(query.CanvasId, cancellationToken);

            if (cachedState != null)
            {
                _logger.LogInformation("Canvas state found in cache for CanvasId {CanvasId}. Returning cached state.", query.CanvasId);
                return cachedState;
            }

            _logger.LogInformation("Canvas state not found in cache for CanvasId {CanvasId}. Proceeding to build state.", query.CanvasId);
        } else {
            _logger.LogInformation("Ignoring cache for CanvasId {CanvasId} due to request flag.", query.CanvasId);
        }

        return null;
    }

/*
    private async void IndexColorData(Canvas canvas, IEnumerable<Pixel> latestPixels, Dictionary<Color, byte> colorToIndexMap, byte defaultColorIndex){
        int canvasSize = canvas.Width * canvas.Height;
        byte[] indexedPixelData = new byte[canvasSize];
        Array.Fill(indexedPixelData, defaultColorIndex); // Pre-fill with background

        foreach (Pixel pixel in latestPixels)
        {
            int index = pixel.Coordinate.Y * canvas.Width + pixel.Coordinate.X;

            if (index >= 0 && index < canvasSize)
            {
                if (colorToIndexMap.TryGetValue(pixel.Color, out byte colorIndex))
                {
                    indexedPixelData[index] = colorIndex;
                }
                else
                {
                    // This indicates a pixel event with a color not in the canvas's current palette - data integrity issue?
                    _logger.LogWarning("Pixel {PixelId} at ({X},{Y}) on Canvas {CanvasId} has color {Color} which is not in the current Palette {PaletteId}. Using default index {DefaultIndex}.",
                        pixel.Id, pixel.Coordinate.X, pixel.Coordinate.Y, canvas.Id, pixel.Color, palette.Id, defaultColorIndex);
      
                    indexedPixelData[index] = defaultColorIndex;
                }
            }
            else
            {
                _logger.LogWarning("Pixel {PixelId} coordinate ({X},{Y}) is out of calculated bounds for Canvas {CanvasId} ({Width}x{Height}). Skipping.",
                    pixel.Id, pixel.Coordinate.X, pixel.Coordinate.Y, canvas.Id, canvas.Width, canvas.Height);
            }
        }
    }
    */
}
