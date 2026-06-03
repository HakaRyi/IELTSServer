using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Practice.Domain.Entities;

public class GeneratedPassage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Topic { get; set; } = null!;

    public double TargetBand { get; set; }

    public string EnglishContent { get; set; } = null!;

    public string VietnameseTranslation { get; set; } = null!;

    public List<string> UsedLexicalItemIds { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
