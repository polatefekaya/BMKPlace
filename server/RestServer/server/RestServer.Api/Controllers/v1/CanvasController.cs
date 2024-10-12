using System;
using System.Threading.Channels;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using RestServer.Application.Interfaces;
using RestServer.Application.Interfaces.Infrastructure;
using RestServer.Domain.Data.Entities;

namespace RestServer.Api.Controllers.v1;

[ApiVersion("1.0")]
public class CanvasController : CustomControllerBase
{
    private ILogger<CanvasController> _logger;
    private readonly IRequestService _requestService;

    public CanvasController(ILogger<CanvasController> logger, IRequestService requestService){
        _logger = logger;
        _requestService = requestService;
    }

    [HttpGet]
    public async Task<ActionResult<CanvasEntity>> Get(int id){
        try{
            CanvasEntity response = await _requestService.PublishRPC<CanvasEntity, int>(id, "canvas-get", true);
            return Ok(response);
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CanvasEntity>>> GetByUser(int userId){
        try{
            IEnumerable<CanvasEntity> response = await _requestService.PublishRPC<IEnumerable<CanvasEntity>, int>(userId, "canvas-getbyuserid", true);
            return Ok(response);
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CanvasEntity>>> GetByDate(){
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody]CanvasEntity entity){
        _requestService.Publish<CanvasEntity, CanvasEntity>(entity, "canvas-add");
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(){
        
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteByUser(){
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(){
        return Ok();
    }

}
