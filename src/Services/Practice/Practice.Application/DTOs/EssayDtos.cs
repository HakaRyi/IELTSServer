using System.Text.Json.Serialization;

namespace Practice.Application.DTOs;

public class ScoreEssayRequest
{
    public string Prompt { get; set; } = null!;
    public string EssayText { get; set; } = null!;
}

public class EssayResponse
{
    public string Id { get; set; } = null!;
    public string Prompt { get; set; } = null!;
    public string EssayText { get; set; } = null!;
    public int WordCount { get; set; }
    public double OverallBand { get; set; }
    public CriterionScoreDto TaskResponse { get; set; } = new();
    public CriterionScoreDto CoherenceCohesion { get; set; } = new();
    public CriterionScoreDto LexicalResource { get; set; } = new();
    public CriterionScoreDto GrammaticalRange { get; set; } = new();
    public string GeneralFeedback { get; set; } = string.Empty;
    public List<string> Improvements { get; set; } = new();
    public List<string> UsedTargetVocabulary { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CriterionScoreDto
{
    public double Band { get; set; }
    public string Comment { get; set; } = string.Empty;
}

// ─── DTO nhận từ Groq ─────────────────────────────────────────────────────────

public class EssayScoreResult
{
    [JsonPropertyName("overallBand")]
    public double OverallBand { get; set; }

    [JsonPropertyName("taskResponse")]
    public GroqCriterion TaskResponse { get; set; } = new();

    [JsonPropertyName("coherenceCohesion")]
    public GroqCriterion CoherenceCohesion { get; set; } = new();

    [JsonPropertyName("lexicalResource")]
    public GroqCriterion LexicalResource { get; set; } = new();

    [JsonPropertyName("grammaticalRange")]
    public GroqCriterion GrammaticalRange { get; set; } = new();

    [JsonPropertyName("generalFeedback")]
    public string GeneralFeedback { get; set; } = string.Empty;

    [JsonPropertyName("improvements")]
    public List<string> Improvements { get; set; } = new();

    [JsonPropertyName("usedTargetVocabulary")]
    public List<string> UsedTargetVocabulary { get; set; } = new();
}

public class GroqCriterion
{
    [JsonPropertyName("band")]
    public double Band { get; set; }

    [JsonPropertyName("comment")]
    public string Comment { get; set; } = string.Empty;
}
