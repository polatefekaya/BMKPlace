using System;
using Microsoft.AspNetCore.Mvc;

namespace RestServer.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiController]
public class CustomControllerBase : ControllerBase
{

}
