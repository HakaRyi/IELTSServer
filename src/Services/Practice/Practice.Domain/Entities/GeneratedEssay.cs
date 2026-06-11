using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Practice.Domain.Entities;

public class GeneratedEssay
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Prompt { get; set; } = null!;       // đề bài
    public string EssayText { get; set; } = null!;     // bài user viết
    public int WordCount { get; set; }

    public double OverallBand { get; set; }

    public CriterionScore TaskResponse { get; set; } = new();
    public CriterionScore CoherenceCohesion { get; set; } = new();
    public CriterionScore LexicalResource { get; set; } = new();
    public CriterionScore GrammaticalRange { get; set; } = new();

    public string GeneralFeedback { get; set; } = string.Empty;
    public List<string> Improvements { get; set; } = new();
    public List<string> UsedTargetVocabulary { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CriterionScore
{
    public double Band { get; set; }
    public string Comment { get; set; } = string.Empty;
}
