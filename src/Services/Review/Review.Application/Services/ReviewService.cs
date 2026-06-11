using Review.Application.DTOs;
using Review.Application.Interfaces;
using Review.Domain.Entities;

namespace Review.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewCardRepository _repo;

    public ReviewService(IReviewCardRepository repo)
    {
        _repo = repo;
    }

    public async Task<ReviewCardDto> EnrollAsync(string userId, EnrollRequest request)
    {
        // Idempotent: nếu user đã enroll từ này thì trả về card cũ
        var existing = await _repo.GetByLexicalItemIdAsync(userId, request.LexicalItemId);
        if (existing != null) return ToDto(existing);

        var card = new ReviewCard
        {
            UserId = userId,
            LexicalItemId = request.LexicalItemId,
            Word = request.Word,
            Type = request.Type,
            Definition = request.Definition,
            Example = request.Example,
            Topics = request.Topics,
            NextReviewAt = DateTime.UtcNow
        };

        await _repo.CreateAsync(card);
        return ToDto(card);
    }

    public async Task<List<ReviewCardDto>> GetDueAsync(string userId)
    {
        var cards = await _repo.GetDueAsync(userId, DateTime.UtcNow);
        return cards.Select(ToDto).ToList();
    }

    public async Task<ReviewCardDto> RateAsync(string userId, string cardId, int quality)
    {
        var card = await _repo.GetByIdAsync(userId, cardId)
            ?? throw new KeyNotFoundException($"Card {cardId} not found.");

        ApplySm2(card, quality);
        card.LastReviewedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(card);
        return ToDto(card);
    }

    public async Task<StatsDto> GetStatsAsync(string userId)
    {
        var (total, due, mastered) = await _repo.GetStatsAsync(userId, DateTime.UtcNow);
        return new StatsDto { Total = total, DueToday = due, Mastered = mastered };
    }

    public async Task<List<ReviewCardDto>> GetAllAsync(string userId)
    {
        var cards = await _repo.GetAllAsync(userId);
        return cards.Select(ToDto).ToList();
    }

    public Task DeleteAsync(string userId, string cardId) => _repo.DeleteAsync(userId, cardId);

    // ─── SM-2 Algorithm ──────────────────────────────────────────────────────
    private static void ApplySm2(ReviewCard card, int quality)
    {
        quality = Math.Clamp(quality, 1, 5);

        if (quality < 3)
        {
            card.Repetitions = 0;
            card.Interval = 1;
        }
        else
        {
            card.Repetitions++;
            card.Interval = card.Repetitions switch
            {
                1 => 1,
                2 => 6,
                _ => (int)Math.Round(card.Interval * card.EaseFactor)
            };
            card.EaseFactor = Math.Max(1.3,
                card.EaseFactor + 0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02));
        }

        card.NextReviewAt = DateTime.UtcNow.AddDays(card.Interval);
    }

    private static ReviewCardDto ToDto(ReviewCard c) => new()
    {
        Id = c.Id,
        LexicalItemId = c.LexicalItemId,
        Word = c.Word,
        Type = c.Type,
        Definition = c.Definition,
        Example = c.Example,
        Topics = c.Topics,
        Repetitions = c.Repetitions,
        EaseFactor = c.EaseFactor,
        Interval = c.Interval,
        NextReviewAt = c.NextReviewAt,
        EnrolledAt = c.EnrolledAt,
        LastReviewedAt = c.LastReviewedAt
    };
}
