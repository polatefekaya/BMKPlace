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

namespace BMKPlace.Application.Features.Authentication.Commands.VerifyOtp;

public sealed class VerifyOtpHandler : ICommandHandler<VerifyOtpCommand, AuthenticationResponse>
{
    private readonly IOtpService _otpService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<VerifyOtpHandler> _logger;

    public VerifyOtpHandler(
        IOtpService otpService,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ILogger<VerifyOtpHandler> logger)
    {
        _otpService = otpService;
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async ValueTask<AuthenticationResponse> Handle(VerifyOtpCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling VerifyOtpCommand for Email {Email}", command.Email); // Sanitize email for logging?

        // 1. --- Input Validation ---
        ValidateCommandInput(command);
        string normalizedEmail = command.Email.ToLowerInvariant().Trim();

        // 2. --- Verify OTP ---
        _logger.LogDebug("Verifying OTP for {Email}", normalizedEmail);
        bool isOtpValid = await _otpService.VerifyOtpAsync(normalizedEmail, command.Otp, cancellationToken);
        if (!isOtpValid)
        {
            _logger.LogWarning("Invalid or expired OTP provided for {Email}", normalizedEmail);
            // Provide a generic error to avoid hinting if email exists or OTP was just wrong
            throw new ApplicationValidationException("Invalid verification code provided.");
        }
        _logger.LogInformation("OTP verified successfully for {Email}.", normalizedEmail);

        // 3. --- Find or Create User ---
        // OTP is valid, proceed to find or register the user.
        ApplicationUser? user = await _userManager.FindByEmailAsync(normalizedEmail);

        if (user is null)
        {
            // User doesn't exist - this OTP verification acts as registration completion
            _logger.LogInformation("User not found for {Email}. Creating new user.", normalizedEmail);
            user = new ApplicationUser
            {
                UserName = normalizedEmail, // Typically use email as username, or generate one
                Email = normalizedEmail,
                EmailConfirmed = true // OTP verification confirms email ownership
                // Set other default properties if needed
            };

            IdentityResult result = await _userManager.CreateAsync(user); // Password not set here - maybe passwordless or set later?

            if (!result.Succeeded)
            {
                // Handle user creation errors (e.g., duplicate username/email if race condition, invalid chars)
                var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
                _logger.LogError("Failed to create user {Email}: {@Errors}", normalizedEmail, errors);
                // Throw specific exception based on Identity errors if possible
                throw new ApplicationValidationException("Failed to create user.", errors);
            }
            _logger.LogInformation("New user created successfully with ID {UserId} for email {Email}", user.Id, normalizedEmail);
        }
        else
        {
            // User exists - this is effectively a login completion via OTP
            _logger.LogInformation("Existing user {UserId} found for email {Email}.", user.Id, normalizedEmail);

            // Optional checks for existing users:
            if (!user.EmailConfirmed)
            {
                // If somehow OTP was generated for unconfirmed email, confirm it now.
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("Confirmed email for existing user {UserId}.", user.Id);
            }
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("User {UserId} attempted OTP login while locked out.", user.Id);
                throw new ApplicationValidationException("Account is locked out."); // Use specific exception?
            }
            // Potentially update last login time, etc.
        }

        // 4. --- Generate Authentication Token ---
        // We have a valid user (either existing or newly created)
        _logger.LogDebug("Generating authentication token for User {UserId}", user.Id);
        // Fetch roles if needed for token service
        IList<string> roles = await _userManager.GetRolesAsync(user);
        string token = await _tokenService.GenerateTokenAsync(user.Id, user.UserName!, roles); // Assuming UserName is not null

        _logger.LogInformation("Authentication successful for User {UserId}. Token generated.", user.Id);

        // 5. --- Return Authentication Response ---
        return new AuthenticationResponse(
            AccessToken: token,
            UserId: user.Id, // User ID is int
            Username: user.UserName! // Assuming UserName is not null
        );
    }

    private void ValidateCommandInput(VerifyOtpCommand command)
    {
        _logger.LogDebug("Validating VerifyOtpCommand input...");
        Dictionary<string, List<string>> validationErrors = new();

        if (string.IsNullOrWhiteSpace(command.Email))
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.Email), "Email address is required.");
        else if (!new EmailAddressAttribute().IsValid(command.Email)) 
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.Email), "Email address is not valid.");

        if (string.IsNullOrWhiteSpace(command.Otp))
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.Otp), "OTP code is required.");
        // Maybe add length check for OTP if it's fixed length? e.g., Regex.IsMatch(command.Otp, @"^\d{6}$")

        ValidationHelpers.ThrowIfErrorsExist(validationErrors, "Validation failed for OTP verification request.", _logger, command);
        _logger.LogDebug("Input validation passed for VerifyOtpCommand.");
    }
}
