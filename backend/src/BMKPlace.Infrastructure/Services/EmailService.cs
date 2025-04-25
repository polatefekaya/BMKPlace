using System;
using BMKPlace.Application.Contracts.Abstractions.Infrastructure;
using BMKPlace.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BMKPlace.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptionsMonitor<EmailOptions> emailOptions, ILogger<EmailService> logger)
    {
        _emailOptions = emailOptions.CurrentValue;
        _logger = logger;
    }

    public async Task SendOtpEmailAsync(
        string recipientEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(recipientEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        if (string.IsNullOrWhiteSpace(_emailOptions.FromAddress) || string.IsNullOrWhiteSpace(_emailOptions.FromName) || string.IsNullOrWhiteSpace(_emailOptions.SmtpHost))
        {
            _logger.LogError("Email configuration (FromAddress, FromName, SmtpHost) is incomplete. Cannot send email.");
            // Depending on requirements, throw or just log and return? Throwing is safer.
            throw new InvalidOperationException("Email service is not configured correctly.");
        }

        _logger.LogInformation("Attempting to send OTP email to {RecipientEmail} with subject '{Subject}'", recipientEmail, subject);

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailOptions.FromName, _emailOptions.FromAddress));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = subject;

            // Set body as plain text based on interface contract
            message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = body
            };

            using var client = new SmtpClient();

            SecureSocketOptions socketOptions = _emailOptions.UseStartTls
                ? SecureSocketOptions.StartTlsWhenAvailable
                : SecureSocketOptions.Auto;

            _logger.LogDebug("Connecting to SMTP server {SmtpHost}:{SmtpPort} using {SocketOptions}...", _emailOptions.SmtpHost, _emailOptions.SmtpPort, socketOptions);

            await client.ConnectAsync(_emailOptions.SmtpHost, _emailOptions.SmtpPort, socketOptions, cancellationToken);

            if (!string.IsNullOrWhiteSpace(_emailOptions.SmtpUsername) && !string.IsNullOrWhiteSpace(_emailOptions.SmtpPassword))
            {
                _logger.LogDebug("Authenticating with SMTP server using username {SmtpUsername}...", _emailOptions.SmtpUsername);
                await client.AuthenticateAsync(_emailOptions.SmtpUsername, _emailOptions.SmtpPassword, cancellationToken);
            }
            else
            {
                 _logger.LogDebug("No SMTP credentials provided, attempting anonymous connection.");
            }

            _logger.LogDebug("Sending email...");
            await client.SendAsync(message, cancellationToken);
            _logger.LogInformation("Email successfully sent to {RecipientEmail}.", recipientEmail);

            await client.DisconnectAsync(true, cancellationToken);
            _logger.LogDebug("Disconnected from SMTP server.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {RecipientEmail}.", recipientEmail);
            // Re-throw or handle as appropriate for the application's error strategy
            // In RequestOtpHandler, we decided to log and continue, but maybe throwing here is better
            // to signal a definitive infrastructure failure. Let's re-throw for now.
            throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
        }
    }
}
