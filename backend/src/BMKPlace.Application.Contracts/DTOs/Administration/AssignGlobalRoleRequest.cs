using System.ComponentModel.DataAnnotations;

namespace BMKPlace.Application.Contracts.DTOs.Administration;

public sealed record AssignGlobalRoleRequest([Required] string RoleName);
