using Review.Domain.Entities;

namespace Review.Domain.Interfaces;

// Lưu ý: interface dùng thực tế nằm ở Review.Application.Interfaces.IReviewCardRepository.
public interface IReviewCardRepository
{
    Task<ReviewCard> CreateAsync(ReviewCard card);
    Task<ReviewCard?> GetByIdAsync(string userId, string id);
    Task<ReviewCard?> GetByLexicalItemIdAsync(string userId, string lexicalItemId);
    Task UpdateAsync(ReviewCard card);
    Task DeleteAsync(string userId, string id);
    Task<List<ReviewCard>> GetDueAsync(string userId, DateTime asOf);
    Task<List<ReviewCard>> GetAllAsync(string userId);
    Task<(int Total, int Due, int Mastered)> GetStatsAsync(string userId, DateTime asOf);
}
