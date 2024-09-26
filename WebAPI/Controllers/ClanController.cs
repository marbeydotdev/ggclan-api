using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1")]
public class ClanController : ControllerBase
{
    [HttpGet("private")]
    [Authorize]
    public IActionResult Private()
    {
        return Ok(new
        {
            Message = "Hello from a private endpoint!"
        });
    }

    [HttpGet("private-scoped")]
    [Authorize("clan")]
    public IActionResult Scoped()
    {
        return Ok(new
        {
            Message = "Hello from a private-scoped endpoint! clan"
        });
    }
    
}