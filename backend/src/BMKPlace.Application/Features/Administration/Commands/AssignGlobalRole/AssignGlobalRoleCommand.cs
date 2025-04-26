using Mediator;

namespace BMKPlace.Application.Features.Administration.Commands.AssignGlobalRole;

public sealed record AssignGlobalRoleCommand(int UserId, string RoleName) : ICommand;