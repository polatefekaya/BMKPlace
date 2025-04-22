using System;

namespace BMKPlace.Application.Contracts.Abstractions.Authentication;

public interface IOtpService
{
    Task<string> GenerateAndStoreOtpAsync(string identifier, CancellationToken cancellationToken = default); Task<bool> VerifyOtpAsync(string identifier, string otp, CancellationToken cancellationToken = default);
}
