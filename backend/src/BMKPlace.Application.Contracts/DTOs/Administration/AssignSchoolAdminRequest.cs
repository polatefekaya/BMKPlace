using System.ComponentModel.DataAnnotations;

namespace BMKPlace.Application.Contracts.DTOs.Administration;

public sealed record AssignSchoolAdminRequest([Required] int UserId);
