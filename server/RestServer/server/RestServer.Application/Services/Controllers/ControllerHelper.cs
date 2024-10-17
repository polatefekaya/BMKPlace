using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestServer.Application.Interfaces;
using RestServer.Domain.Data.Entities;

namespace RestServer.Application.Services.Controllers;

public class ControllerHelper : ControllerBase 
{
    private readonly ILogger<ControllerHelper> _logger;

    public ControllerHelper(ILogger<ControllerHelper> logger){
        _logger = logger;
    }
    public async Task<ActionResult<T>> Run<T>(Func<Task<T>> operation)
    {
        _logger.LogDebug("Operation is started");
        try{
            T result = await operation.Invoke();
            return Ok(result);
        }
        catch(OperationCanceledException){
            _logger.LogDebug("Request timed out");
            return StatusCode(504, "Request timed out");
        }
        catch(ArgumentNullException){
            return StatusCode(500, "Null return");
        }
        catch(Exception ex){
            _logger.LogError(ex, "An unexpected error occurred");
            return StatusCode(500, "An unexpected error occurred");
        }
    }
    public async Task<IActionResult> Run(Action operation)
    {
        _logger.LogDebug("Operation is started");
        try{
            operation.Invoke();
            return Ok();
        }
        catch(ArgumentNullException){
            return StatusCode(500, "Null return");
        }
        catch(Exception ex){
            _logger.LogError(ex, "An unexpected error occurred");
            return StatusCode(500, "An unexpected error occurred");
        }
    }
}
