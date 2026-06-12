using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KnowledgeBase.Domain.Entities;

public class LexicalItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    /// <summary>Chủ sở hữu — userId từ JWT (Auth service).</summary>
    public string UserId { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string Type { get; set; } = null!;

    public List<string> Topics { get; set; } = new();

    public List<Meaning> Meanings { get; set; } = new();

    public List<string> Synonyms { get; set; } = new();

    public List<string> Antonyms { get; set; } = new();

    public string PersonalNotes { get; set; } = string.Empty;

    /// <summary>Ngày tra/lưu từ — dùng để nhóm theo ngày trong app.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Meaning
{
    public string Definition { get; set; } = null!;

    public string Connotation { get; set; } = "Neutral";

    /// <summary>Độ mạnh sắc thái 1-5 (1 = rất nhẹ, 5 = cực mạnh). 0 = không xác định (data cũ).</summary>
    public int Intensity { get; set; }

    /// <summary>Giải thích sắc thái, so sánh với từ gần nghĩa (vd: like &lt; love &lt; adore &lt; be obsessed with).</summary>
    public string IntensityNote { get; set; } = string.Empty;

    public List<string> Examples { get; set; } = new();
}
