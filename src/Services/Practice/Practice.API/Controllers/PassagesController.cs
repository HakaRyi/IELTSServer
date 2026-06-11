using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Application.DTOs;
using Practice.Application.Interfaces;
using Shared.Core.Authentication;

namespace Practice.API.Controllers;

[Authorize]
[ApiController]
[Route("api/passages")]
public class PassagesController : ControllerBase
{
    private readonly IPassageService _service;

    public PassagesController(IPassageService service)
    {
        _service = service;
    }

    private string UserId => User.GetRequiredUserId();

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] GeneratePassageRequest request)
    {
        var result = await _service.GenerateAndSavePassageAsync(UserId, request);
        return Ok(result);
    }

    [HttpGet("topic/{topic}")]
    public async Task<IActionResult> GetByTopic(string topic)
    {
        var result = await _service.GetPassagesByTopicAsync(UserId, topic);
        return Ok(result);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int limit = 20)
    {
        var result = await _service.GetRecentAsync(UserId, Math.Clamp(limit, 1, 50));
        return Ok(result);
    }
}
