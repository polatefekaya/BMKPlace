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
        _logger.LogInformation("Received OTP request for email: {Email}", request.Email);
        var command = new RequestOtpCommand { Email = request.Email };
        await _mediator.Send(command);
        return Accepted();
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthenticationResponse>> LoginWithOtp([FromBody] VerifyOtpRequest request) // Reusing DTO
    {
        _logger.LogInformation("Received LoginWithOtp request for email: {Email}", request.Email);
        var command = new LoginWithOtpCommand { Email = request.Email, Otp = request.Otp };
        AuthenticationResponse response = await _mediator.Send(command);
        return Ok(response);
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
        var command = new CompleteRegistrationCommand { Email = request.Email, Otp = request.Otp, SchoolId = request.SchoolId };
        AuthenticationResponse response = await _mediator.Send(command);
        return Ok(response);
    }
}
