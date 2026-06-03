using Microsoft.AspNetCore.Mvc;
using Practice.Application.DTOs;
using Practice.Application.Interfaces;

namespace Practice.API.Controllers;

[ApiController]
[Route("api/speaking")]
public class SpeakingController : ControllerBase
{
    private readonly ISpeakingService _service;

    public SpeakingController(ISpeakingService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] GenerateSpeakingRequest request)
    {
        var result = await _service.GenerateAndSaveAsync(request);
        return Ok(result);
    }

    [HttpGet("topic/{topic}")]
    public async Task<IActionResult> GetByTopic(string topic)
    {
        var result = await _service.GetByTopicAsync(topic);
        return Ok(result);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int limit = 20)
    {
        var result = await _service.GetRecentAsync(Math.Clamp(limit, 1, 50));
        return Ok(result);
    }
}
