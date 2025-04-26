using System.ComponentModel.DataAnnotations;

namespace BMKPlace.Application.Contracts.DTOs.Authentication;

public sealed record CompleteRegistrationRequest(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string Otp, // Consider adding length validation attribute if fixed length

    [Required] // School ID is now mandatory for registration
    int SchoolId
    // Add other registration fields if needed (e.g., chosen username if not email)
);
