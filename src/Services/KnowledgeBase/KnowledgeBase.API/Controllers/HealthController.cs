using Microsoft.AspNetCore.Mvc;

namespace KnowledgeBase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { Status = "Healthy", Service = "KnowledgeBase.API" });
}
