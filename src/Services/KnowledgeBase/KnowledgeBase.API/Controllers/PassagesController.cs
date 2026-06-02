using System.Threading.Tasks;
using KnowledgeBase.Application.DTOs;
using KnowledgeBase.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KnowledgeBase.API.Controllers;

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
    public async Task<IActionResult> Generate([FromBody] GeneratePassageDto dto)
    {
        var result = await _service.GenerateAndSavePassageAsync(dto);
        return Ok(result);
    }

    [HttpGet("topic/{topic}")]
    public async Task<IActionResult> GetByTopic(string topic)
    {
        var result = await _service.GetPassagesByTopicAsync(topic);
        return Ok(result);
    }
}