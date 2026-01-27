using Microsoft.AspNetCore.Mvc;

namespace AcademiaLounge.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    [HttpGet("auth-header")]
    public IActionResult AuthHeader()
        => Ok(new { Authorization = Request.Headers["Authorization"].ToString() });
}
