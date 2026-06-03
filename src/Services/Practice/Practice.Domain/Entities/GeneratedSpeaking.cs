using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Practice.Domain.Entities;

public class GeneratedSpeaking
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Topic { get; set; } = null!;

    public double TargetBand { get; set; }

    public List<SpeakingQA> Part1 { get; set; } = new();

    public SpeakingPart2 Part2 { get; set; } = new();

    public List<SpeakingQA> Part3 { get; set; } = new();

    public List<string> UsedVocabulary { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SpeakingQA
{
    public string Question { get; set; } = null!;
    public string SampleAnswer { get; set; } = null!;
}

public class SpeakingPart2
{
    public string CueCard { get; set; } = null!;
    public List<string> Points { get; set; } = new();
    public string SampleAnswer { get; set; } = null!;
}
