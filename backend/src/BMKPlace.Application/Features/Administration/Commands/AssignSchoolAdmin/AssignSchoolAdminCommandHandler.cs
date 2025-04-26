using System;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Domain.Entities;
using BMKPlace.Infrastructure.Identity;
using BMKPlace.Infrastructure.Persistence;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.Administration.Commands.AssignSchoolAdmin;

public sealed class AssignSchoolAdminCommandHandler : ICommandHandler<AssignSchoolAdminCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ISchoolRepository _schoolRepository;
    private readonly ApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignSchoolAdminCommandHandler> _logger;

    private const string SchoolAdminRole = "SchoolAdmin";

    public AssignSchoolAdminCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        ISchoolRepository schoolRepository,
        ApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ILogger<AssignSchoolAdminCommandHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _schoolRepository = schoolRepository;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(AssignSchoolAdminCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling AssignSchoolAdminCommand for UserId: {UserId}, SchoolId: {SchoolId}", command.UserId, command.SchoolId);

        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User with ID {command.UserId} not found.");
        }

        if (!await _schoolRepository.ExistsAsync(command.SchoolId, cancellationToken))
        {
            throw new NotFoundException($"School with ID {command.SchoolId} not found.");
        }

        // Ensure the SchoolAdmin role exists (consider seeding roles on startup)
        if (!await _roleManager.RoleExistsAsync(SchoolAdminRole))
        {
             _logger.LogError("'{SchoolAdminRole}' role does not exist. Cannot assign school admin.", SchoolAdminRole);
             throw new ApplicationValidationException($"Required role '{SchoolAdminRole}' not found in the system.");
        }

        // Check if assignment already exists
        bool alreadyAssigned = await _dbContext.SchoolAdminAssignments
            .AnyAsync(saa => saa.UserId == command.UserId && saa.SchoolId == command.SchoolId, cancellationToken);

        if (alreadyAssigned)
        {
             _logger.LogWarning("User {UserId} is already an admin for School {SchoolId}. No action taken.", command.UserId, command.SchoolId);
             return Unit.Value; // Treat as success
        }

        // Add user to the "SchoolAdmin" role if they aren't already in it
        if (!await _userManager.IsInRoleAsync(user, SchoolAdminRole))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, SchoolAdminRole);
            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
                _logger.LogError("Failed to add user {UserId} to role {SchoolAdminRole} during school admin assignment: {@Errors}", command.UserId, SchoolAdminRole, errors);
                throw new ApplicationValidationException($"Failed to assign '{SchoolAdminRole}' role.", errors);
            }
             _logger.LogInformation("Added user {UserId} to '{SchoolAdminRole}' role.", command.UserId, SchoolAdminRole);
        }

        // Create and add the assignment record
        var assignment = SchoolAdministratorAssignment.Create(command.UserId, command.SchoolId);
        await _dbContext.SchoolAdminAssignments.AddAsync(assignment, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully assigned User {UserId} as admin for School {SchoolId}", command.UserId, command.SchoolId);
        return Unit.Value;
    }
}
