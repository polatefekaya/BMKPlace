using System;
using System.ComponentModel.DataAnnotations;
using BMKPlace.Application.Common.Helpers;
using BMKPlace.Application.Contracts.Abstractions.Authentication;
using BMKPlace.Application.Contracts.DTOs.Authentication;
using BMKPlace.Infrastructure.Identity;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.Authentication.Commands.LoginWithOtp;

public class LoginWithOtpHandler : ICommandHandler<LoginWithOtpCommand, AuthenticationResponse>
{
    private readonly IOtpService _otpService;
    private readonly UserManager<ApplicationUser> _userManager; // Inject UserManager
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginWithOtpHandler> _logger;

    public LoginWithOtpHandler(
        IOtpService otpService,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ILogger<LoginWithOtpHandler> logger)
    {
        _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<AuthenticationResponse> Handle(LoginWithOtpCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling LoginWithOtpCommand for Email {Email}", command.Email); // Sanitize email?

        // 1. --- Input Validation ---
        ValidateCommandInput(command);
        string normalizedEmail = command.Email.ToLowerInvariant().Trim();

        // 2. --- Verify OTP ---
        _logger.LogDebug("Verifying OTP for login attempt: {Email}", normalizedEmail);
        bool isOtpValid = await _otpService.VerifyOtpAsync(normalizedEmail, command.Otp, cancellationToken);
        if (!isOtpValid)
        {
            _logger.LogWarning("Invalid or expired OTP provided for login attempt for {Email}", normalizedEmail);
            throw new ApplicationValidationException("Invalid verification code provided."); // Generic error
        }
        _logger.LogInformation("OTP verified successfully for {Email}.", normalizedEmail);
        // Successful verification consumes the OTP via IOtpService implementation.

        // 3. --- Find Existing User ---
        // This command should FAIL if the user is *not* already registered.
        _logger.LogDebug("Finding existing user for {Email}", normalizedEmail);
        ApplicationUser? user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            _logger.LogWarning("Login attempt failed: User not found for email {Email}", normalizedEmail);
            // Throw NotFound or a validation exception suggesting registration?
            // Let's use validation to guide user.
            throw new ApplicationValidationException("Email address not registered. Please complete registration first.", new Dictionary<string, string[]> { { "email", new[] { "Email not registered." } } });
        }
        _logger.LogInformation("Existing user {UserId} found for email {Email}.", user.Id, normalizedEmail);

        // 4. --- Check User Status (Example) ---
        if (!user.EmailConfirmed)
        {
            // Should not happen if OTP was sent, but good defensive check.
             _logger.LogWarning("User {UserId} attempted OTP login with unconfirmed email.", user.Id);
             // Maybe re-confirm email here? Or throw? Let's throw for now.
              throw new ApplicationValidationException("Email requires confirmation.");
        }
        if (await _userManager.IsLockedOutAsync(user))
        {
            _logger.LogWarning("User {UserId} attempted OTP login while locked out.", user.Id);
            throw new ApplicationValidationException("Account is locked out.");
        }
        // Potentially update last login time via UserManager or another service after successful token generation.

        // 5. --- Generate Authentication Token ---
        _logger.LogDebug("Generating authentication token for User {UserId}", user.Id);
        IList<string> roles = await _userManager.GetRolesAsync(user);
        string token = await _tokenService.GenerateTokenAsync(user.Id, user.UserName!, roles); // Assuming UserName not null

        _logger.LogInformation("Login successful for User {UserId}. Token generated.", user.Id);

        // 6. --- Return Authentication Response ---
        return new AuthenticationResponse(
            AccessToken: token,
            UserId: user.Id, // int ID
            Username: user.UserName!
        );
    }

    private void ValidateCommandInput(LoginWithOtpCommand command)
    {
        _logger.LogDebug("Validating LoginWithOtpCommand input...");
        Dictionary<string, List<string>> validationErrors = new();

        if (string.IsNullOrWhiteSpace(command.Email))
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.Email), "Email address is required.");
        else if (!new EmailAddressAttribute().IsValid(command.Email)) // Simple check
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.Email), "Email address is not valid.");

        if (string.IsNullOrWhiteSpace(command.Otp))
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.Otp), "OTP code is required.");

        ValidationHelpers.ThrowIfErrorsExist(validationErrors, "Validation failed for OTP request.", _logger, command);
        _logger.LogDebug("Input validation passed for LoginWithOtpCommand.");
    }
}
