using Review.Application.DTOs;

namespace Review.Application.Interfaces;

public interface IReviewService
{
    Task<ReviewCardDto> EnrollAsync(string userId, EnrollRequest request);
    Task<List<ReviewCardDto>> GetDueAsync(string userId);
    Task<ReviewCardDto> RateAsync(string userId, string cardId, int quality);
    Task<StatsDto> GetStatsAsync(string userId);
    Task<List<ReviewCardDto>> GetAllAsync(string userId);
    Task DeleteAsync(string userId, string cardId);
}
