using System;

namespace BMKPlace.Application.Contracts.Abstractions.Infrastructure;

public interface IEmailService
{
    Task SendOtpEmailAsync(string recipientEmail, string subject, string body, CancellationToken cancellationToken = default);
}
