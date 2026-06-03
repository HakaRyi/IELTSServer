using Microsoft.AspNetCore.Mvc;
using Review.Application.DTOs;
using Review.Application.Interfaces;

namespace Review.API.Controllers;

[ApiController]
[Route("api/review")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _service;
    public ReviewController(IReviewService service) => _service = service;

    // POST /api/review/enroll
    [HttpPost("enroll")]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest request)
    {
        var card = await _service.EnrollAsync(request);
        return Ok(card);
    }

    // GET /api/review/due
    [HttpGet("due")]
    public async Task<IActionResult> GetDue()
        => Ok(await _service.GetDueAsync());

    // POST /api/review/{cardId}/rate
    [HttpPost("{cardId}/rate")]
    public async Task<IActionResult> Rate(string cardId, [FromBody] RateRequest request)
    {
        try
        {
            var card = await _service.RateAsync(cardId, request.Quality);
            return Ok(card);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    // GET /api/review/stats
    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
        => Ok(await _service.GetStatsAsync());

    // GET /api/review/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    // DELETE /api/review/{cardId}
    [HttpDelete("{cardId}")]
    public async Task<IActionResult> Delete(string cardId)
    {
        await _service.DeleteAsync(cardId);
        return NoContent();
    }
}
