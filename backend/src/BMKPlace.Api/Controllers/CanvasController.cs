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
        _logger.LogInformation("Request received for GetCanvasInfo for CanvasId: {CanvasId}", canvasId);
        try
        {
            var query = new GetCanvasInfoQuery { CanvasId = canvasId };
            CanvasInfoDto result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Canvas not found for GetCanvasInfo. CanvasId: {CanvasId}", canvasId);
            var error = new ErrorDto(ex.GetType().Name, "Not Found", StatusCodes.Status404NotFound, ex.Message);
            return NotFound(error);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Unexpected error during GetCanvasInfo for CanvasId: {CanvasId}", canvasId);
             var error = new ErrorDto("ServerError", "Internal Server Error", StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving canvas info.");
             return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
    }

    [HttpGet("{canvasId:int}/state")] // Route: GET /api/canvases/1/state?ignoreCache=true
    [AllowAnonymous]
    [ProducesResponseType(typeof(CanvasStateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CanvasStateDto>> GetCanvasState([FromRoute] int canvasId, [FromQuery] bool ignoreCache = false)
    {
         _logger.LogInformation("Request received for GetCanvasState for CanvasId: {CanvasId}, IgnoreCache: {IgnoreCache}", canvasId, ignoreCache);
         try
        {
            var query = new GetCanvasStateQuery { CanvasId = canvasId, IgnoreCache = ignoreCache };
            CanvasStateDto result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
             _logger.LogWarning(ex, "Canvas not found for GetCanvasState. CanvasId: {CanvasId}", canvasId);
             var error = new ErrorDto(ex.GetType().Name, "Not Found", StatusCodes.Status404NotFound, ex.Message);
             return NotFound(error);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Unexpected error during GetCanvasState for CanvasId: {CanvasId}", canvasId);
             var error = new ErrorDto("ServerError", "Internal Server Error", StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving canvas state.");
             return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
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
        _logger.LogInformation("Received request to create canvas: {CanvasName}", request.Name);
        try
        {
            // Map DTO to Command (including TimeSpan conversion)
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

            // Return 201 Created with a link to the GetCanvasInfo endpoint and the result
            return CreatedAtAction(nameof(GetCanvasInfo), new { canvasId = result.Id }, result);
        }
        catch (ApplicationValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed creating canvas: {CanvasName}", request.Name);
            var error = new ErrorDto(ex.GetType().Name, "Validation Error", StatusCodes.Status400BadRequest, ex.Message);
             // Potentially include ex.Errors details if needed by client
            return BadRequest(error);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Unexpected error creating canvas: {CanvasName}", request.Name);
             var error = new ErrorDto("ServerError", "Internal Server Error", StatusCodes.Status500InternalServerError, "An unexpected error occurred while creating the canvas.");
             return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
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
         _logger.LogInformation("Received request to update settings for CanvasId: {CanvasId}", canvasId);
        try
        {
            var command = new UpdateCanvasSettingsCommand
            {
                CanvasId = canvasId, // Get ID from route
                Name = request.Name,
                Width = request.Width,
                Height = request.Height,
                DefaultCooldown = TimeSpan.FromSeconds(request.DefaultCooldownSeconds),
                ColorPaletteId = request.ColorPaletteId
            };

            await _mediator.Send(command);

            // Return 204 No Content for successful PUT/update operations
            return NoContent();
        }
        catch (NotFoundException ex)
        {
             _logger.LogWarning(ex, "Canvas not found for UpdateCanvasSettings. CanvasId: {CanvasId}", canvasId);
             var error = new ErrorDto(ex.GetType().Name, "Not Found", StatusCodes.Status404NotFound, ex.Message);
             return NotFound(error);
        }
        catch (ApplicationValidationException ex)
        {
             _logger.LogWarning(ex, "Validation failed updating canvas settings: CanvasId {CanvasId}", canvasId);
             var error = new ErrorDto(ex.GetType().Name, "Validation Error", StatusCodes.Status400BadRequest, ex.Message);
             return BadRequest(error);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Unexpected error updating canvas settings: CanvasId {CanvasId}", canvasId);
             var error = new ErrorDto("ServerError", "Internal Server Error", StatusCodes.Status500InternalServerError, "An unexpected error occurred while updating the canvas.");
             return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
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
        _logger.LogInformation("Received request to activate CanvasId: {CanvasId}", canvasId);
        try
        {
            var command = new ActivateCanvasCommand(canvasId);
            await _mediator.Send(command);
            return NoContent(); // Success
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Canvas not found for ActivateCanvas. CanvasId: {CanvasId}", canvasId);
            var error = new ErrorDto(ex.GetType().Name, "Not Found", StatusCodes.Status404NotFound, ex.Message);
            return NotFound(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error activating canvas: CanvasId {CanvasId}", canvasId);
            var error = new ErrorDto("ServerError", "Internal Server Error", StatusCodes.Status500InternalServerError, "An unexpected error occurred while activating the canvas.");
            return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
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
         _logger.LogInformation("Received request to deactivate CanvasId: {CanvasId}", canvasId);
        try
        {
            var command = new DeactivateCanvasCommand(canvasId);
            await _mediator.Send(command);
            return NoContent(); // Success
        }
        catch (NotFoundException ex)
        {
             _logger.LogWarning(ex, "Canvas not found for DeactivateCanvas. CanvasId: {CanvasId}", canvasId);
             var error = new ErrorDto(ex.GetType().Name, "Not Found", StatusCodes.Status404NotFound, ex.Message);
             return NotFound(error);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Unexpected error deactivating canvas: CanvasId {CanvasId}", canvasId);
             var error = new ErrorDto("ServerError", "Internal Server Error", StatusCodes.Status500InternalServerError, "An unexpected error occurred while deactivating the canvas.");
             return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
    }
}
