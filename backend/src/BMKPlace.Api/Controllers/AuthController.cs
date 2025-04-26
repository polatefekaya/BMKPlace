using System;
using System.Net.Mime;
using BMKPlace.Application.Contracts.DTOs.Authentication;
using BMKPlace.Application.Contracts.DTOs.Common;
using BMKPlace.Application.Features.Authentication.Commands.CompleteRegistration;
using BMKPlace.Application.Features.Authentication.Commands.LoginWithOtp;
using BMKPlace.Application.Features.Authentication.Commands.RequestOtp;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;

namespace BMKPlace.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // Routes will be /api/auth/*
[Produces(MediaTypeNames.Application.Json)] // Default produces JSON
[Consumes(MediaTypeNames.Application.Json)] // Default consumes JSON
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger){
        _mediator = mediator;
        _logger = logger;
    }

    
    [HttpPost("request-otp")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpRequest request)
    {
        _logger.LogInformation("Received OTP request for email: {Email}", request.Email); // Consider sanitizing email log
        try
        {
            var command = new RequestOtpCommand { Email = request.Email };
            await _mediator.Send(command);

            // Return 202 Accepted: The request is accepted, processing initiated (email sending is async).
            // Doesn't guarantee email delivery, just that the request was valid and OTP generated/stored.
            return Accepted();
        }
        catch (ApplicationValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for RequestOtp: {Email}", request.Email);
            // Use ErrorDto structure
            var error = new ErrorDto(ex.GetType().Name, "Validation Error", StatusCodes.Status400BadRequest, ex.Message);
            // Consider adding ex.Errors dictionary to a structured response if needed
            return BadRequest(error);
        }
        catch (InvalidOperationException ex) // Catch specific exceptions from the handler if needed
        {
             _logger.LogError(ex, "Operation failed during RequestOtp: {Email}", request.Email);
             var error = new ErrorDto(ex.GetType().Name, "Operation Failed", StatusCodes.Status500InternalServerError, "Could not process OTP request at this time.");
             return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
        catch (Exception ex) // Catch-all for unexpected errors
        {
            _logger.LogError(ex, "Unexpected error during RequestOtp: {Email}", request.Email);
            var error = new ErrorDto("ServerError", "Internal Server Error", StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthenticationResponse>> LoginWithOtp([FromBody] VerifyOtpRequest request) // Reusing DTO
    {
         _logger.LogInformation("Received LoginWithOtp request for email: {Email}", request.Email);
        try
        {
            var command = new LoginWithOtpCommand { Email = request.Email, Otp = request.Otp };
            AuthenticationResponse response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (ApplicationValidationException ex) // Covers invalid OTP, lockout, unconfirmed email etc. from handler
        {
             _logger.LogWarning(ex, "Validation/Login failed for LoginWithOtp: {Email}", request.Email);
             var error = new ErrorDto(ex.GetType().Name, "Login Failed", StatusCodes.Status400BadRequest, ex.Message);
             return BadRequest(error);
        }
        catch (NotFoundException ex) // Should not happen if handler uses AppValidationException for "not registered"
        {
            _logger.LogWarning(ex, "Not found during LoginWithOtp: {Email}", request.Email);
             var error = new ErrorDto(ex.GetType().Name, "Not Found", StatusCodes.Status404NotFound, ex.Message);
             return NotFound(error);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Unexpected error during LoginWithOtp: {Email}", request.Email);
             var error = new ErrorDto("ServerError", "Internal Server Error", StatusCodes.Status500InternalServerError, "An unexpected error occurred during login.");
             return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status409Conflict)] //for "already registered"
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthenticationResponse>> CompleteRegistration(
        [FromBody] CompleteRegistrationRequest request) 
    {
        _logger.LogInformation("Received CompleteRegistration request for email: {Email}, SchoolId: {SchoolId}", request.Email, request.SchoolId);
        try
        {
            // Map Request DTO to Command
            var command = new CompleteRegistrationCommand
            {
                Email = request.Email,
                Otp = request.Otp,
                SchoolId = request.SchoolId
            };

            AuthenticationResponse response = await _mediator.Send(command);
            // Return Ok or CreatedAtAction if preferred
            return Ok(response);
        }
        catch (ApplicationValidationException ex)
        {
             _logger.LogWarning(ex, "Validation/Registration failed for CompleteRegistration: {Email}", request.Email);
             var error = new ErrorDto(ex.GetType().Name, "Registration Failed", StatusCodes.Status400BadRequest, ex.Message);
             // Check if specific message indicates "already registered" and maybe return 409 Conflict?
             if (ex.Message.Contains("already registered", StringComparison.OrdinalIgnoreCase))
             {
                 error = error with { Status = StatusCodes.Status409Conflict, Title = "Conflict" };
                 return Conflict(error);
             }
             return BadRequest(error);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Unexpected error during CompleteRegistration: {Email}", request.Email);
             var error = new ErrorDto("ServerError", "Internal Server Error", StatusCodes.Status500InternalServerError, "An unexpected error occurred during registration.");
             return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
    }
}
