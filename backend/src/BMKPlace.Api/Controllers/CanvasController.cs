using System;
using BMKPlace.Application.Contracts.DTOs.Canvas;
using BMKPlace.Application.Contracts.DTOs.Common;
using BMKPlace.Application.Features.PixelCanvas.Commands.ActivateCanvas;
using BMKPlace.Application.Features.PixelCanvas.Commands.CreateCanvas;
using BMKPlace.Application.Features.PixelCanvas.Commands.DeactivateCanvas;
using BMKPlace.Application.Features.PixelCanvas.Commands.UpdateCanvasSettings;
using BMKPlace.Application.Features.PixelCanvas.Queries.GetCanvasInfo;
using BMKPlace.Application.Features.PixelCanvas.Queries.GetCanvasState;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;

namespace BMKPlace.Api.Controllers;

public class CanvasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CanvasController> _logger;

    public CanvasController(IMediator mediator, ILogger<CanvasController> logger){
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{canvasId:int}/info")] // Route: GET /api/canvases/1/info
    [AllowAnonymous] // Example: Allow anonymous access to basic info
    [ProducesResponseType(typeof(CanvasInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CanvasInfoDto>> GetCanvasInfo([FromRoute] int canvasId)
    {
        _logger.LogInformation("Handling request for GetCanvasInfo for CanvasId: {CanvasId}", canvasId);
        var query = new GetCanvasInfoQuery { CanvasId = canvasId };

        CanvasInfoDto result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{canvasId:int}/state")] // Route: GET /api/canvases/1/state?ignoreCache=true
    [AllowAnonymous]
    [ProducesResponseType(typeof(CanvasStateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CanvasStateDto>> GetCanvasState([FromRoute] int canvasId, [FromQuery] bool ignoreCache = false)
    {
        _logger.LogInformation("Handling request for GetCanvasState for CanvasId: {CanvasId}, IgnoreCache: {IgnoreCache}", canvasId, ignoreCache);
        var query = new GetCanvasStateQuery { CanvasId = canvasId, IgnoreCache = ignoreCache };

        CanvasStateDto result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // AUTHORIZATION: Only Admins can create canvases
    [ProducesResponseType(typeof(CanvasInfoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CanvasInfoDto>> CreateCanvas([FromBody] CreateCanvasRequest request)
    {
        _logger.LogInformation("Handling request to create canvas: {CanvasName}", request.Name);
        var command = new CreateCanvasCommand
        {
            Name = request.Name,
            Width = request.Width,
            Height = request.Height,
            DefaultCooldown = TimeSpan.FromSeconds(request.DefaultCooldownSeconds),
            ColorPaletteId = request.ColorPaletteId,
            SchoolId = request.SchoolId
        };
        
        CanvasInfoDto result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCanvasInfo), new { canvasId = result.Id }, result);
    }

    [HttpPut("{canvasId:int}")] // Route: PUT /api/canvases/1
    [Authorize(Roles = "Admin")] // AUTHORIZATION
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCanvasSettings([FromRoute] int canvasId, [FromBody] UpdateCanvasSettingsRequest request)
    {
        _logger.LogInformation("Handling request to update settings for CanvasId: {CanvasId}", canvasId);
        var command = new UpdateCanvasSettingsCommand
        {
            CanvasId = canvasId,
            Name = request.Name,
            Width = request.Width,
            Height = request.Height,
            DefaultCooldown = TimeSpan.FromSeconds(request.DefaultCooldownSeconds),
            ColorPaletteId = request.ColorPaletteId
        };
        
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{canvasId:int}/activate")] // Route: POST /api/canvases/1/activate
    [Authorize(Roles = "Admin")] // AUTHORIZATION
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActivateCanvas([FromRoute] int canvasId)
    {
        _logger.LogInformation("Handling request to activate CanvasId: {CanvasId}", canvasId);
        var command = new ActivateCanvasCommand(canvasId);
        // Let middleware handle NotFoundException
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{canvasId:int}/deactivate")] // Route: POST /api/canvases/1/deactivate
    [Authorize(Roles = "Admin")] // AUTHORIZATION
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeactivateCanvas([FromRoute] int canvasId)
    {
        _logger.LogInformation("Handling request to deactivate CanvasId: {CanvasId}", canvasId);
        var command = new DeactivateCanvasCommand(canvasId);
        
        await _mediator.Send(command);
        return NoContent();
    }
}
