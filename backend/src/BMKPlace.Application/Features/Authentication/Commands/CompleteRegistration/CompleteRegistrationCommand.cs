using System;
using BMKPlace.Application.Contracts.DTOs.Authentication;
using Mediator;

namespace BMKPlace.Application.Features.Authentication.Commands.CompleteRegistration;

public sealed record CompleteRegistrationCommand : ICommand<AuthenticationResponse>
{
    public required string Email { get; init; }
    public required string Otp { get; init; }
    public required int SchoolId {get; init;}
}
