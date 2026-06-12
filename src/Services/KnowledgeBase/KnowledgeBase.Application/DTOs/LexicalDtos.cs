namespace KnowledgeBase.Application.DTOs;

// Kết quả phân tích từ Gemini (chưa lưu)
public class WordAnalysisDto
{
    public string Value { get; set; } = null!;
    public string Type { get; set; } = null!;
    public List<string> Topics { get; set; } = new();
    public List<MeaningDto> Meanings { get; set; } = new();
    public List<string> Synonyms { get; set; } = new();
    public List<string> Antonyms { get; set; } = new();
}

public class MeaningDto
{
    public string Definition { get; set; } = null!;
    public string Connotation { get; set; } = "Neutral";
    /// <summary>Độ mạnh sắc thái 1-5 (vd: like=2, love=4, be obsessed with=5).</summary>
    public int Intensity { get; set; }
    public string IntensityNote { get; set; } = string.Empty;
    public List<string> Examples { get; set; } = new();
}

// Kết quả tra từ trả về cho mobile
public class LookupResultDto
{
    public string Source { get; set; } = null!;   // "vault" hoặc "generated"
    public string? Id { get; set; }                // có Id nếu đã ở trong kho
    public WordAnalysisDto Data { get; set; } = null!;
}

// Request lưu từ vào kho
public class CreateLexicalItemRequest
{
    public string Value { get; set; } = null!;
    public string Type { get; set; } = null!;
    public List<string> Topics { get; set; } = new();
    public List<MeaningDto> Meanings { get; set; } = new();
    public List<string> Synonyms { get; set; } = new();
    public List<string> Antonyms { get; set; } = new();
    public string PersonalNotes { get; set; } = string.Empty;
}

public class UpdateLexicalItemRequest : CreateLexicalItemRequest { }

// Thống kê số từ theo chủ đề (cho tab Chủ đề)
public class TopicStatDto
{
    public string Topic { get; set; } = null!;
    public long Count { get; set; }
}

// Gợi ý từ khi search
public class SuggestItemDto
{
    public string Id { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Type { get; set; } = null!;
    public List<string> Topics { get; set; } = new();
}

// Kết quả phân trang
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public long Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}