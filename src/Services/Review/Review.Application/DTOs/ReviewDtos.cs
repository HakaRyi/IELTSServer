namespace Review.Application.DTOs;

public class EnrollRequest
{
    public string LexicalItemId { get; set; } = null!;
    public string Word { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Definition { get; set; } = null!;
    public string Example { get; set; } = string.Empty;
    public List<string> Topics { get; set; } = new();
}

public class RateRequest
{
    /// <summary>1=Again  3=Good  5=Easy</summary>
    public int Quality { get; set; }
}

public class ReviewCardDto
{
    public string Id { get; set; } = null!;
    public string LexicalItemId { get; set; } = null!;
    public string Word { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Definition { get; set; } = null!;
    public string Example { get; set; } = null!;
    public List<string> Topics { get; set; } = new();
    public int Repetitions { get; set; }
    public double EaseFactor { get; set; }
    public int Interval { get; set; }
    public DateTime NextReviewAt { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? LastReviewedAt { get; set; }
}

public class StatsDto
{
    public int Total { get; set; }
    public int DueToday { get; set; }
    public int Mastered { get; set; }   // interval >= 21 days
}
