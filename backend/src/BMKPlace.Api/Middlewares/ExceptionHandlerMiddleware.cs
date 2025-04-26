using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using System.Net; 
using System.Text.Json; 

namespace BMKPlace.Api.Middleware;

public sealed class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    private static readonly IReadOnlyDictionary<Type, HttpStatusCode> ExceptionStatusCodes =
        new Dictionary<Type, HttpStatusCode>
        {
            { typeof(ApplicationValidationException), HttpStatusCode.BadRequest },
            { typeof(DomainValidationException), HttpStatusCode.BadRequest }, 
            { typeof(NotFoundException), HttpStatusCode.NotFound },
            { typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized }, 
            // Add other specific exception types and their desired status codes here
        };

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }


    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request. Path: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }


    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode = DetermineStatusCode(exception);

        var problemDetails = CreateProblemDetails(context, exception, statusCode);

        context.Response.ContentType = "application/problem+json"; // RFC 7807 media type
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        string jsonResponse = JsonSerializer.Serialize(problemDetails, jsonOptions);

        return context.Response.WriteAsync(jsonResponse);
    }

    private static HttpStatusCode DetermineStatusCode(Exception exception)
    {
        // Check our specific mappings first
        if (ExceptionStatusCodes.TryGetValue(exception.GetType(), out HttpStatusCode statusCode))
        {
            return statusCode;
        }

        return HttpStatusCode.InternalServerError;
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception, HttpStatusCode statusCode)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Instance = context.Request.Path, 
            Title = GetTitleForStatusCode(statusCode),
            Detail = GetDetail(exception)
        };

        if (exception is ApplicationValidationException validationException && validationException.Errors.Count > 0)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
            problemDetails.Title = "Validation Error"; 
            problemDetails.Detail = "One or more validation errors occurred."; 
        }
        else if (exception is DomainValidationException domainException)
        {
             problemDetails.Title = "Domain Rule Violation"; 
        }


        return problemDetails;
    }

    private static string GetTitleForStatusCode(HttpStatusCode statusCode) =>
        statusCode switch
        {
            HttpStatusCode.BadRequest => "Bad Request",
            HttpStatusCode.Unauthorized => "Unauthorized",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.NotFound => "Not Found",
            HttpStatusCode.Conflict => "Conflict",
            HttpStatusCode.InternalServerError => "Internal Server Error",
            _ => "An error occurred"
        };


    private string GetDetail(Exception exception)
    {
        if (_env.IsDevelopment())
        {
            return exception.ToString(); 
        }

        return "An unexpected error occurred. Please try again later.";
    }
}