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

namespace BMKPlace.Application.Features.Authentication.Commands.CompleteRegistration;

public class CompleteRegistrationHandler : ICommandHandler<CompleteRegistrationCommand, AuthenticationResponse>
{
    private readonly IOtpService _otpService;
    private readonly UserManager<ApplicationUser> _userManager; // Inject UserManager
    private readonly ITokenService _tokenService;
    private readonly ILogger<CompleteRegistrationHandler> _logger;

    public CompleteRegistrationHandler(
        IOtpService otpService,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ILogger<CompleteRegistrationHandler> logger)
    {
        _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async ValueTask<AuthenticationResponse> Handle(CompleteRegistrationCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CompleteRegistrationCommand for Email {Email}", command.Email);

        ValidateCommandInput(command);
        string normalizedEmail = command.Email.ToLowerInvariant().Trim();

        _logger.LogDebug("Verifying OTP for potential registration: {Email}", normalizedEmail);
        bool isOtpValid = await _otpService.VerifyOtpAsync(normalizedEmail, command.Otp, cancellationToken);
        if (!isOtpValid)
        {
            _logger.LogWarning("Invalid or expired OTP provided during registration attempt for {Email}", normalizedEmail);
            throw new ApplicationValidationException("Invalid verification code provided."); // Keep error generic
        }
        _logger.LogInformation("OTP verified successfully for {Email}.", normalizedEmail);

        _logger.LogDebug("Checking if user already exists for {Email}", normalizedEmail);
        ApplicationUser? existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existingUser != null)
        {
            _logger.LogWarning("Registration attempt failed: Email {Email} is already registered.", normalizedEmail);
            throw new ApplicationValidationException("This email address is already registered. Please try logging in.", new Dictionary<string, string[]> { { "email", new[] { "Email already registered." } } });
        }

        _logger.LogInformation("Creating new user for {Email}.", normalizedEmail);
        var user = new ApplicationUser
        {
            UserName = normalizedEmail, // Consider prompting for a username earlier or generating one? Using email for now.
            Email = normalizedEmail,
            EmailConfirmed = true 
            // TODO: Set any other required fields for ApplicationUser
        };

        IdentityResult result = await _userManager.CreateAsync(user); // Create user WITHOUT password initially

        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
            _logger.LogError("Failed to create user {Email} via UserManager: {@Errors}", normalizedEmail, errors);
            // Convert Identity errors to our application validation exception format
            var validationErrors = new Dictionary<string, List<string>>();
            foreach (var error in result.Errors)
            {
                // Try to map Identity error codes (e.g., "DuplicateUserName") to property names ("email" or "username")
                string key = error.Code.Contains("Email") ? "email" : error.Code.Contains("UserName") ? "username" : "general";
                ValidationHelpers.AddValidationError(validationErrors, key, error.Description);
            }
            ValidationHelpers.ThrowIfErrorsExist(validationErrors, "User registration failed.", _logger, command);
            
        }
        _logger.LogInformation("New user created successfully with ID {UserId} for email {Email}", user.Id, normalizedEmail);

        // TODO: Handle password setting. Passwordless via OTP? Magic Link? Set password later?
        // For now, user is created and email is confirmed.


        _logger.LogDebug("Generating authentication token for newly registered User {UserId}", user.Id);
        // Get roles (likely empty for new user unless defaults are assigned)
        IList<string> roles = await _userManager.GetRolesAsync(user);
        string token = await _tokenService.GenerateTokenAsync(user.Id, user.UserName!, roles);

        _logger.LogInformation("Registration and authentication successful for User {UserId}. Token generated.", user.Id);

        return new AuthenticationResponse(
            AccessToken: token,
            UserId: user.Id, 
            Username: user.UserName!
        );
    }

    private void ValidateCommandInput(CompleteRegistrationCommand command)
    {
        _logger.LogDebug("Validating CompleteRegistrationCommand input...");
        Dictionary<string, List<string>> validationErrors = new();

        if (string.IsNullOrWhiteSpace(command.Email))
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.Email), "Email address is required.");
        else if (!new EmailAddressAttribute().IsValid(command.Email)) // Simple check
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.Email), "Email address is not valid.");

        if (string.IsNullOrWhiteSpace(command.Otp))
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.Otp), "OTP code is required.");
            
        ValidationHelpers.ThrowIfErrorsExist(validationErrors, "Validation failed for OTP request.", _logger, command);
        
        _logger.LogDebug("Input validation passed for CompleteRegistrationCommand.");
    }
}
