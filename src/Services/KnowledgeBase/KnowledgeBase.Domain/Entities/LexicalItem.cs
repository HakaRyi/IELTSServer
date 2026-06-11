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
}

public class Meaning
{
    public string Definition { get; set; } = null!;
    
    public string Connotation { get; set; } = "Neutral";
    
    public List<string> Examples { get; set; } = new();
}