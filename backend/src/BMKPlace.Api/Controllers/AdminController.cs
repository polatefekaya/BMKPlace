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
        _logger.LogInformation("Handling request to assign global role '{RoleName}' to UserId: {UserId}", request.RoleName, userId);

        var command = new AssignGlobalRoleCommand(userId, request.RoleName);
        await _mediator.Send(command);

        return NoContent();
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
        _logger.LogInformation("Handling request to assign UserId: {UserId} as admin for SchoolId: {SchoolId}", request.UserId, schoolId);

        var command = new AssignSchoolAdminCommand(request.UserId, schoolId);
        await _mediator.Send(command);

        return NoContent();
    }
}