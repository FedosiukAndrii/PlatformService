using Microsoft.AspNetCore.Mvc;

namespace PlatformService.Controllers;

[ApiController]
[Route("[controller]")]
public class Controller() : ControllerBase
{
    [HttpGet(Name = "Get")]
    public IActionResult Get()
    {
        return Ok();
    }
}
