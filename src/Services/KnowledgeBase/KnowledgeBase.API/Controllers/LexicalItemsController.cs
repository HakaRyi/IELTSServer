using KnowledgeBase.Application.DTOs;
using KnowledgeBase.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Core.Authentication;

namespace KnowledgeBase.API.Controllers;

[Authorize]
[ApiController]
[Route("api/lexical")]
public class LexicalItemsController : ControllerBase
{
    private readonly ILexicalItemService _service;

    public LexicalItemsController(ILexicalItemService service)
    {
        _service = service;
    }

    private string UserId => User.GetRequiredUserId();

    // GET /api/lexical/lookup?word=resilient
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return BadRequest("Word is required.");

        var result = await _service.LookupAsync(UserId, word.Trim());
        return Ok(result);
    }

    // GET /api/lexical/suggest?q=res&limit=8
    [HttpGet("suggest")]
    public async Task<IActionResult> Suggest([FromQuery] string q, [FromQuery] int limit = 8)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(Array.Empty<object>());
        var result = await _service.SearchAsync(UserId, q, Math.Clamp(limit, 1, 20));
        return Ok(result);
    }

    // GET /api/lexical?topic=Environment&page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> GetVault(
        [FromQuery] string? topic,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _service.GetVaultAsync(UserId, topic, page, pageSize);
        return Ok(result);
    }

    // GET /api/lexical/topics
    [HttpGet("topics")]
    public async Task<IActionResult> GetTopics()
    {
        var topics = await _service.GetTopicsAsync(UserId);
        return Ok(topics);
    }

    // GET /api/lexical/topics/stats — [{topic, count}] cho tab Chủ đề
    [HttpGet("topics/stats")]
    public async Task<IActionResult> GetTopicStats()
    {
        var stats = await _service.GetTopicStatsAsync(UserId);
        return Ok(stats);
    }

    // GET /api/lexical/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await _service.GetByIdAsync(UserId, id);
        return item == null ? NotFound() : Ok(item);
    }

    // POST /api/lexical
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLexicalItemRequest request)
    {
        var item = await _service.CreateAsync(UserId, request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    // PUT /api/lexical/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateLexicalItemRequest request)
    {
        var ok = await _service.UpdateAsync(UserId, id, request);
        return ok ? NoContent() : NotFound();
    }

    // DELETE /api/lexical/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var ok = await _service.DeleteAsync(UserId, id);
        return ok ? NoContent() : NotFound();
    }
}
