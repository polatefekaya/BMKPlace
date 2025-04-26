using System;
using System.ComponentModel.DataAnnotations;
using BMKPlace.Application.Contracts.DTOs.Common;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using BMKPlace.Application.Features.Pixels.Queries.GetLatestPixelForCoordinate;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;

namespace BMKPlace.Api.Controllers;

public class PixelsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PixelsController> _logger;

    public PixelsController(IMediator mediator, ILogger<PixelsController> logger){
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("coordinate")] // Route: GET /api/canvases/1/pixels/coordinate?x=10&y=20
    [ProducesResponseType(typeof(PixelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)] 
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)] 
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PixelDto>> GetPixelAtCoordinate(
        [FromRoute] int canvasId,
        [FromQuery][Range(0, int.MaxValue)] int x, 
        [FromQuery][Range(0, int.MaxValue)] int y)
    {
        _logger.LogInformation("Handling HTTP request for GetPixelAtCoordinate for CanvasId: {CanvasId} at ({X},{Y})", canvasId, x, y);

        var query = new GetLatestPixelForCoordinateQuery
        {
            CanvasId = canvasId,
            X = x,
            Y = y
        };

        PixelDto? result = await _mediator.Send(query);

        if (result is null)
        {
            _logger.LogInformation("No pixel found via HTTP request at ({X},{Y}) for CanvasId: {CanvasId}", x, y, canvasId);
            return NotFound();
        }

        return Ok(result);
    }
}
