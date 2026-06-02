using System.Collections.Generic;

namespace KnowledgeBase.Application.DTOs;

public class GeneratePassageDto
{
    public string Topic { get; set; } = null!;
    public double TargetBand { get; set; }
    public List<string> LexicalItemIds { get; set; } = new();
}