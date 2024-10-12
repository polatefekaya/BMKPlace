using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RestServer.Application.Interfaces.Infrastructure;
using RestServer.Domain.Data.Entities;

namespace RestServer.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    public class UserController : CustomControllerBase
    {
        private ILogger<UserController> _logger;
        private readonly IMessageClient _messageClient;
        private readonly string _className = "User";
        public UserController(ILogger<UserController> logger, IMessageClient messageClient){
            _logger = logger;
            _messageClient = messageClient;
        }

        [HttpGet]
        public async Task<ActionResult<UserEntity>> Get(){
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody]UserEntity entity){
            _logger.LogInformation("UserController Add is started");
            _messageClient.SendMessage<UserEntity, UserEntity>(entity, $"{_className}-{nameof(Add)}");
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(){
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(){
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetByCanvas(){
            return Ok();
        }
    }
}
