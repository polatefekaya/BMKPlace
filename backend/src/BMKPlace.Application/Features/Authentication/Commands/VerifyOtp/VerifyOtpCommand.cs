using System;
using BMKPlace.Application.Contracts.DTOs.Authentication;
using Mediator;

namespace BMKPlace.Application.Features.Authentication.Commands.VerifyOtp;

public sealed record VerifyOtpCommand : ICommand<AuthenticationResponse>
{
    public required string Email { get; init; }
    public required string Otp { get; init; } 
}
