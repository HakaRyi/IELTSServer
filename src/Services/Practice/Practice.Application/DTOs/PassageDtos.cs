namespace Practice.Application.DTOs;

public class GeneratePassageRequest
{
    public string Topic { get; set; } = null!;
    public double TargetBand { get; set; }
    public List<string> LexicalItemIds { get; set; } = new();
}

public class PassageResponse
{
    public string Id { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public double TargetBand { get; set; }
    public string EnglishContent { get; set; } = null!;
    public string VietnameseTranslation { get; set; } = null!;
    public List<string> UsedLexicalItemIds { get; set; } = new();
    public List<string> UsedVocabulary { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
