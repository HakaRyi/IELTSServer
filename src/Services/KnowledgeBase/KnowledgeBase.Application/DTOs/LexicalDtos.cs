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

// Kết quả phân trang
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public long Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}