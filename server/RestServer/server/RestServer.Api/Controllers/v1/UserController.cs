using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RestServer.Domain.Data.Entities;

namespace RestServer.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    public class UserController : CustomControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<UserEntity>> Get(){
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Add(){
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
