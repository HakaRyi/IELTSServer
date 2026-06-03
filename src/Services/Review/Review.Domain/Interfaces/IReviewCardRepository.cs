using Review.Domain.Entities;

namespace Review.Domain.Interfaces;

public interface IReviewCardRepository
{
    Task<ReviewCard> CreateAsync(ReviewCard card);
    Task<ReviewCard?> GetByIdAsync(string id);
    Task<ReviewCard?> GetByLexicalItemIdAsync(string lexicalItemId);
    Task UpdateAsync(ReviewCard card);
    Task DeleteAsync(string id);
    Task<List<ReviewCard>> GetDueAsync(DateTime asOf);
    Task<List<ReviewCard>> GetAllAsync();
    Task<(int Total, int Due, int Mastered)> GetStatsAsync(DateTime asOf);
}
