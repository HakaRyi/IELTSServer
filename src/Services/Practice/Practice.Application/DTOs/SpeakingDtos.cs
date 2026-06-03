using System.Text.Json.Serialization;

namespace Practice.Application.DTOs;

public class GenerateSpeakingRequest
{
    public string Topic { get; set; } = null!;
    public double TargetBand { get; set; }
    /// <summary>Từ vựng muốn luyện tập (tuỳ chọn — lấy từ kho theo chủ đề)</summary>
    public List<string> VocabularyWords { get; set; } = new();
}

public class SpeakingResponse
{
    public string Id { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public double TargetBand { get; set; }
    public List<SpeakingQADto> Part1 { get; set; } = new();
    public SpeakingPart2Dto Part2 { get; set; } = new();
    public List<SpeakingQADto> Part3 { get; set; } = new();
    public List<string> UsedVocabulary { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class SpeakingQADto
{
    public string Question { get; set; } = null!;
    public string SampleAnswer { get; set; } = null!;
}

public class SpeakingPart2Dto
{
    public string CueCard { get; set; } = null!;
    public List<string> Points { get; set; } = new();
    public string SampleAnswer { get; set; } = null!;
}

// ─── Internal DTO từ Gemini ───────────────────────────────────────────────────

public class SpeakingGeminiResult
{
    [JsonPropertyName("part1")]
    public List<GeminiQA> Part1 { get; set; } = new();

    [JsonPropertyName("part2")]
    public GeminiPart2 Part2 { get; set; } = new();

    [JsonPropertyName("part3")]
    public List<GeminiQA> Part3 { get; set; } = new();

    [JsonPropertyName("usedVocabulary")]
    public List<string> UsedVocabulary { get; set; } = new();
}

public class GeminiQA
{
    [JsonPropertyName("question")]
    public string Question { get; set; } = null!;

    [JsonPropertyName("sampleAnswer")]
    public string SampleAnswer { get; set; } = null!;
}

public class GeminiPart2
{
    [JsonPropertyName("cueCard")]
    public string CueCard { get; set; } = null!;

    [JsonPropertyName("points")]
    public List<string> Points { get; set; } = new();

    [JsonPropertyName("sampleAnswer")]
    public string SampleAnswer { get; set; } = null!;
}
