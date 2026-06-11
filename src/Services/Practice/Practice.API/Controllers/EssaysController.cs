using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Application.DTOs;
using Practice.Application.Interfaces;
using Shared.Core.Authentication;

namespace Practice.API.Controllers;

[Authorize]
[ApiController]
[Route("api/essays")]
public class EssaysController : ControllerBase
{
    private readonly IEssayService _service;

    public EssaysController(IEssayService service)
    {
        _service = service;
    }

    private string UserId => User.GetRequiredUserId();

    [HttpPost("score")]
    public async Task<IActionResult> Score([FromBody] ScoreEssayRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EssayText))
            return BadRequest("Bài viết không được để trống.");
        var result = await _service.ScoreAndSaveAsync(UserId, request);
        return Ok(result);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int limit = 20)
    {
        var result = await _service.GetRecentAsync(UserId, Math.Clamp(limit, 1, 50));
        return Ok(result);
    }
}
