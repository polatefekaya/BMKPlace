using BMKPlace.Application.Contracts.DTOs.Administration;
using BMKPlace.Application.Contracts.DTOs.Common;
using BMKPlace.Application.Features.Administration.Commands.AssignGlobalRole;
using BMKPlace.Application.Features.Administration.Commands.AssignSchoolAdmin;
using Mediator;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions; 
using System.Net.Mime;

namespace BMKPlace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IMediator mediator, ILogger<AdminController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("users/{userId:int}/roles")] // POST /api/admin/users/123/roles
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignGlobalRole([FromRoute] int userId, [FromBody] AssignGlobalRoleRequest request)
    {
        _logger.LogInformation("Received request to assign global role '{RoleName}' to UserId: {UserId}", request.RoleName, userId);
        try
        {
            var command = new AssignGlobalRoleCommand(userId, request.RoleName);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
             _logger.LogWarning(ex, "Resource not found during AssignGlobalRole. UserId: {UserId}", userId);
             var error = new ErrorDto(ex.GetType().Name, "Not Found", StatusCodes.Status404NotFound, ex.Message);
             return NotFound(error);
        }
        catch (ApplicationValidationException ex)
        {
             _logger.LogWarning(ex, "Validation failed during AssignGlobalRole. UserId: {UserId}, Role: {RoleName}", userId, request.RoleName);
             var error = new ErrorDto(ex.GetType().Name, "Validation Error", StatusCodes.Status400BadRequest, ex.Message);
             return BadRequest(error);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Unexpected error during AssignGlobalRole. UserId: {UserId}, Role: {RoleName}", userId, request.RoleName);
             var error = new ErrorDto("ServerError", "Internal Server Error", StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
             return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
    }

    [HttpPost("schools/{schoolId:int}/admins")] // POST /api/admin/schools/5/admins
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignSchoolAdmin([FromRoute] int schoolId, [FromBody] AssignSchoolAdminRequest request)
    {
         _logger.LogInformation("Received request to assign UserId: {UserId} as admin for SchoolId: {SchoolId}", request.UserId, schoolId);
        try
        {
            var command = new AssignSchoolAdminCommand(request.UserId, schoolId);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
             _logger.LogWarning(ex, "Resource not found during AssignSchoolAdmin. UserId: {UserId}, SchoolId: {SchoolId}", request.UserId, schoolId);
             var error = new ErrorDto(ex.GetType().Name, "Not Found", StatusCodes.Status404NotFound, ex.Message);
             return NotFound(error);
        }
        catch (ApplicationValidationException ex)
        {
             _logger.LogWarning(ex, "Validation failed during AssignSchoolAdmin. UserId: {UserId}, SchoolId: {SchoolId}", request.UserId, schoolId);
             var error = new ErrorDto(ex.GetType().Name, "Validation Error", StatusCodes.Status400BadRequest, ex.Message);
             return BadRequest(error);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Unexpected error during AssignSchoolAdmin. UserId: {UserId}, SchoolId: {SchoolId}", request.UserId, schoolId);
             var error = new ErrorDto("ServerError", "Internal Server Error", StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
             return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
    }
}