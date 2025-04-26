using System;
using BMKPlace.Infrastructure.Identity;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.Administration.Commands.AssignGlobalRole;

public sealed class AssignGlobalRoleCommandHandler : ICommandHandler<AssignGlobalRoleCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ILogger<AssignGlobalRoleCommandHandler> _logger;

    public AssignGlobalRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        ILogger<AssignGlobalRoleCommandHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(AssignGlobalRoleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling AssignGlobalRoleCommand for UserId: {UserId}, Role: {RoleName}", command.UserId, command.RoleName);

        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User with ID {command.UserId} not found.");
        }

        // Validate RoleName (ensure it's a valid, existing role)
        if (string.IsNullOrWhiteSpace(command.RoleName) || !await _roleManager.RoleExistsAsync(command.RoleName))
        {
            // Maybe define standard roles ("Admin", "SchoolAdmin") somewhere?
            throw new ApplicationValidationException($"Role '{command.RoleName}' is invalid or does not exist.");
        }

        if (await _userManager.IsInRoleAsync(user, command.RoleName))
        {
             _logger.LogWarning("User {UserId} is already in role {RoleName}. No action taken.", command.UserId, command.RoleName);
             // Optionally throw validation exception or just return success
             // throw new ApplicationValidationException($"User is already in role '{command.RoleName}'.");
             return Unit.Value; // Treat as success if already in role
        }

        var result = await _userManager.AddToRoleAsync(user, command.RoleName);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
            _logger.LogError("Failed to add user {UserId} to role {RoleName}: {@Errors}", command.UserId, command.RoleName, errors);
            throw new ApplicationValidationException($"Failed to assign role '{command.RoleName}'.", errors);
        }

        _logger.LogInformation("Successfully assigned role {RoleName} to user {UserId}", command.RoleName, user.Id);
        return Unit.Value;
    }
}
