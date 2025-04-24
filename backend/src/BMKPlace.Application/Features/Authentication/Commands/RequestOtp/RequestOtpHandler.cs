using System;
using System.ComponentModel.DataAnnotations;
using BMKPlace.Application.Common.Helpers;
using BMKPlace.Application.Contracts.Abstractions.Authentication;
using BMKPlace.Application.Contracts.Abstractions.Infrastructure;
using Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.Authentication.Commands.RequestOtp;

public sealed class RequestOtpHandler : ICommandHandler<RequestOtpCommand>
{
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;
    private readonly IUserLookupService _userLookupService; // To check if email exists (optional based on flow)
    private readonly IAllowedDomainChecker _domainChecker; // To check school domain
    private readonly ILogger<RequestOtpHandler> _logger;

    public RequestOtpHandler(
        IOtpService otpService,
        IEmailService emailService,
        IUserLookupService userLookupService,
        IAllowedDomainChecker domainChecker,
        ILogger<RequestOtpHandler> logger)
    {
        _otpService = otpService;
        _emailService = emailService;
        _userLookupService = userLookupService;
        _domainChecker = domainChecker;
        _logger = logger;
    }


    public async ValueTask<Unit> Handle(RequestOtpCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling RequestOtpCommand for Email {Email}", command.Email);
        ValidateCommandInput(command); 

        string normalizedEmail = command.Email.ToLowerInvariant().Trim(); 

        _logger.LogDebug("Checking if domain for email {Email} is allowed", normalizedEmail);
        bool isAllowed = await _domainChecker.IsDomainAllowedAsync(normalizedEmail, cancellationToken);
        if (!isAllowed)
        {
            _logger.LogWarning("OTP Request denied: Email domain for {Email} is not allowed.", normalizedEmail);
            // Throw a specific exception the API can map to a user-friendly error
            throw new ApplicationValidationException("Email domain is not supported for registration or login.");
        }
        _logger.LogInformation("Email domain for {Email} is allowed.", normalizedEmail);

        // 3. --- Optional: Check if User Exists ---
        // Depending on flow: Do you allow OTP requests only for existing users? Or also for registration?
        // If only for login:
        // var userId = await _userLookupService.FindUserIdByIdentifierAsync(normalizedEmail, cancellationToken);
        // if (!userId.HasValue)
        // {
        //    _logger.LogWarning("OTP Request denied: No user found for email {Email}", normalizedEmail);
        //    throw new NotFoundException("User not registered with this email address.");
        // }

        _logger.LogDebug("Generating and storing OTP for {Email}", normalizedEmail);
        string otp;
        try
        {
            otp = await _otpService.GenerateAndStoreOtpAsync(normalizedEmail, cancellationToken);
            _logger.LogInformation("OTP generated successfully for {Email}.", normalizedEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate or store OTP for {Email}", normalizedEmail);
            // Re-throw or handle as appropriate for the application's error strategy
            throw new InvalidOperationException("Could not generate OTP at this time. Please try again later.", ex);
        }


        // 5. --- Send OTP Email ---
        _logger.LogDebug("Attempting to send OTP email to {Email}", normalizedEmail);
        string subject = "BMKPlace Verification Code";
        // TODO: Use a proper email template (HTML?) stored elsewhere
        string body = $"Your verification code is: {otp}\nThis code will expire in a few minutes."; // Example body

        try
        {
            await _emailService.SendOtpEmailAsync(normalizedEmail, subject, body, cancellationToken);
            _logger.LogInformation("OTP email successfully sent to {Email}.", normalizedEmail);
        }
        catch (Exception ex)
        {
            // Log failure but maybe don't fail the whole operation?
            // The OTP is stored; user might retry later or we might have other verification methods.
            // Critical decision: Should email failure prevent perceived success?
            // Let's log error and continue, assuming OTP storage was the critical part.
            _logger.LogError(ex, "Failed to send OTP email to {Email}. OTP was generated but not sent.", normalizedEmail);
            // Optionally: Could try to clean up the stored OTP if sending failed catastrophically? Complex.
        }

        // 6. --- Return Success ---
        // Command handled successfully (OTP stored, email attempted).
        return Unit.Value; // Return Mediator's equivalent of void/Task
    }

    private void ValidateCommandInput(RequestOtpCommand command)
    {
        _logger.LogDebug("Performing input validation...");
        Dictionary<string, List<string>> validationErrors = new();

        if (string.IsNullOrWhiteSpace(command.Email))
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.Email), "Email address is required.");
        else if (!new EmailAddressAttribute().IsValid(command.Email)) 
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.Email), "Email address is not valid.");

        ValidationHelpers.ThrowIfErrorsExist(validationErrors, "Validation failed for OTP request.", _logger, command);
        _logger.LogDebug("Input validation passed.");
    }
}
