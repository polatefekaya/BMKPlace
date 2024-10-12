using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RestServer.Application.Interfaces;
using RestServer.Application.Interfaces.Infrastructure;
using RestServer.Application.Services.Controllers;
using RestServer.Domain.Data.Entities;

namespace RestServer.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    public class UserController : CustomControllerBase
    {
        private ILogger<UserController> _logger;
        private readonly IRequestService _requestService;
        private readonly ControllerHelper _helper;
        public UserController(ILogger<UserController> logger,IRequestService requestService, ControllerHelper helper){
            _logger = logger;
            _requestService = requestService;
            _helper = helper;
        }

        [HttpGet]
        public async Task<ActionResult<UserEntity>> Get(int id){
            return await _helper.Run(async () => 
                await _requestService.PublishRPC<UserEntity, int>(id, "user-get", true)
            );
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody]UserEntity entity){
            return await _helper.Run(() => {
                _requestService.Publish<UserEntity, UserEntity>(entity, "user-add");
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody]UserEntity entity){
            return await _helper.Run(() => {
                _requestService.Publish<UserEntity, UserEntity>(entity, "user-update");
            });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id){
            return await _helper.Run(() => {
                _requestService.Publish<UserEntity, int>(id, "user-delete");
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetByCanvas(int canvasId){
            return await _helper.Run(async () => 
                await _requestService.PublishRPC<IEnumerable<UserEntity>, int>(canvasId, "user-getbycanvasid", true)
            );
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetByPosition(){
            return Ok("Not implemented");
        }
    }
}
