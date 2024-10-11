using System;
using Microsoft.AspNetCore.Mvc;
using RestServer.Application.Interfaces.Infrastructure;
using RestServer.Domain.Data.Entities;

namespace RestServer.Api.Controllers.v1;

[ApiVersion("1.0")]
public class CanvasController : CustomControllerBase
{
    private ILogger<CanvasController> _logger;
    private readonly IMessageClient _messageClient;

    public CanvasController(ILogger<CanvasController> logger, IMessageClient messageClient){
        _logger = logger;
        _messageClient = messageClient;
    }

    [HttpGet]
    public async Task<ActionResult<CanvasEntity>> Get(){
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CanvasEntity>>> GetByUser(){
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CanvasEntity>>> GetByDate(){
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody]CanvasEntity entity){
        _logger.LogInformation("Trying of a message client");
        _messageClient.SendMessage<CanvasEntity>(entity);
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
