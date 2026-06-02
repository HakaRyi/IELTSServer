using KnowledgeBase.Application.DTOs;
using KnowledgeBase.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KnowledgeBase.API.Controllers;

[ApiController]
[Route("api/lexical")]
public class LexicalItemsController : ControllerBase
{
    private readonly ILexicalItemService _service;

    public LexicalItemsController(ILexicalItemService service)
    {
        _service = service;
    }

    // GET /api/lexical/lookup?word=resilient
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return BadRequest("Word is required.");

        var result = await _service.LookupAsync(word.Trim());
        return Ok(result);
    }

    // GET /api/lexical?topic=Environment&page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> GetVault(
        [FromQuery] string? topic,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _service.GetVaultAsync(topic, page, pageSize);
        return Ok(result);
    }

    // GET /api/lexical/topics
    [HttpGet("topics")]
    public async Task<IActionResult> GetTopics()
    {
        var topics = await _service.GetTopicsAsync();
        return Ok(topics);
    }

    // GET /api/lexical/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    // POST /api/lexical
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLexicalItemRequest request)
    {
        var item = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    // PUT /api/lexical/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateLexicalItemRequest request)
    {
        var ok = await _service.UpdateAsync(id, request);
        return ok ? NoContent() : NotFound();
    }

    // DELETE /api/lexical/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}