using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Review.Application.DTOs;
using Review.Application.Interfaces;
using Shared.Core.Authentication;

namespace Review.API.Controllers;

[Authorize]
[ApiController]
[Route("api/review")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _service;
    public ReviewController(IReviewService service) => _service = service;

    private string UserId => User.GetRequiredUserId();

    [HttpPost("enroll")]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest request)
    {
        var card = await _service.EnrollAsync(UserId, request);
        return Ok(card);
    }

    [HttpGet("due")]
    public async Task<IActionResult> GetDue()
        => Ok(await _service.GetDueAsync(UserId));

    [HttpPost("{cardId}/rate")]
    public async Task<IActionResult> Rate(string cardId, [FromBody] RateRequest request)
    {
        try
        {
            var card = await _service.RateAsync(UserId, cardId, request.Quality);
            return Ok(card);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
        => Ok(await _service.GetStatsAsync(UserId));

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync(UserId));

    [HttpDelete("{cardId}")]
    public async Task<IActionResult> Delete(string cardId)
    {
        await _service.DeleteAsync(UserId, cardId);
        return NoContent();
    }
}
