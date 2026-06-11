using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Review.Domain.Entities;

public class ReviewCard
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string LexicalItemId { get; set; } = null!;

    // Snapshot để hiển thị offline, không cần gọi KnowledgeBase mỗi lần
    public string Word { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Definition { get; set; } = null!;   // nghĩa đầu tiên
    public string Example { get; set; } = string.Empty;
    public List<string> Topics { get; set; } = new();

    // SM-2 schedule
    public int Repetitions { get; set; } = 0;
    public double EaseFactor { get; set; } = 2.5;
    public int Interval { get; set; } = 1;          // days
    public DateTime NextReviewAt { get; set; } = DateTime.UtcNow;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastReviewedAt { get; set; }
}
