using BMKPlace.Application.Contracts.DTOs.Authentication;
using Mediator;

namespace BMKPlace.Application.Features.Authentication.Commands.LoginWithOtp;

public sealed record class LoginWithOtpCommand : ICommand<AuthenticationResponse>
{
    public required string Email { get; init; }
    public required string Otp { get; init; }
}
