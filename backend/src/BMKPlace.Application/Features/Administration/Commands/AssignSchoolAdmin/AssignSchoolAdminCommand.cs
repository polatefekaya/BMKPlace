using Mediator;

namespace BMKPlace.Application.Features.Administration.Commands.AssignSchoolAdmin;

public sealed record AssignSchoolAdminCommand(int UserId, int SchoolId) : ICommand;
