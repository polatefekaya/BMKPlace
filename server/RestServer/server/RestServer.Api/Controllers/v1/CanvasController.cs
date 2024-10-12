using System;
using System.Threading.Channels;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using RestServer.Application.Interfaces;
using RestServer.Application.Interfaces.Infrastructure;
using RestServer.Application.Services.Controllers;
using RestServer.Domain.Data.Entities;

namespace RestServer.Api.Controllers.v1;

[ApiVersion("1.0")]
public class CanvasController : CustomControllerBase
{
    private ILogger<CanvasController> _logger;
    private readonly IRequestService _requestService;
    private readonly ControllerHelper _helper;

    public CanvasController(ILogger<CanvasController> logger, IRequestService requestService, ControllerHelper helper){
        _logger = logger;
        _requestService = requestService;
        _helper = helper;
    }

    [HttpGet]
    public async Task<ActionResult<CanvasEntity>> Get(int id){
        return await _helper.Run(async () => 
            await _requestService.PublishRPC<CanvasEntity, int>(id, "canvas-get", true)
        );
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CanvasEntity>>> GetByUser(int userId){
        return await _helper.Run(async () => 
            await _requestService.PublishRPC<IEnumerable<CanvasEntity>, int>(userId, "canvas-get", true)
        );
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CanvasEntity>>> GetByDate(DateTime date){
        return await _helper.Run(async () => 
            await _requestService.PublishRPC<IEnumerable<CanvasEntity>, DateTime>(date, "canvas-getbydate", true)
        );
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
