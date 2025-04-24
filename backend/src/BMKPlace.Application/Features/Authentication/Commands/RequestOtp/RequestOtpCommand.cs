using System;
using Mediator;

namespace BMKPlace.Application.Features.Authentication.Commands.RequestOtp;

public sealed record RequestOtpCommand : ICommand
{
    public required string Email {get; init;}
}
