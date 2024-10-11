using System;
using Microsoft.AspNetCore.Mvc;
using RestServer.Domain.Data.Entities;

namespace RestServer.Api.Controllers.v1;

[ApiVersion("1.0")]
public class CanvasController : CustomControllerBase
{
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
    public async Task<IActionResult> Add(){
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
