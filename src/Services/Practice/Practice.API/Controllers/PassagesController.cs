using Microsoft.AspNetCore.Mvc;
using Practice.Application.DTOs;
using Practice.Application.Interfaces;

namespace Practice.API.Controllers;

[ApiController]
[Route("api/passages")]
public class PassagesController : ControllerBase
{
    private readonly IPassageService _service;

    public PassagesController(IPassageService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] GeneratePassageRequest request)
    {
        var result = await _service.GenerateAndSavePassageAsync(request);
        return Ok(result);
    }

    [HttpGet("topic/{topic}")]
    public async Task<IActionResult> GetByTopic(string topic)
    {
        var result = await _service.GetPassagesByTopicAsync(topic);
        return Ok(result);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int limit = 20)
    {
        var result = await _service.GetRecentAsync(Math.Clamp(limit, 1, 50));
        return Ok(result);
    }
}
