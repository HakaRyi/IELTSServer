using Review.Application.DTOs;

namespace Review.Application.Interfaces;

public interface IReviewService
{
    Task<ReviewCardDto> EnrollAsync(EnrollRequest request);
    Task<List<ReviewCardDto>> GetDueAsync();
    Task<ReviewCardDto> RateAsync(string cardId, int quality);
    Task<StatsDto> GetStatsAsync();
    Task<List<ReviewCardDto>> GetAllAsync();
    Task DeleteAsync(string cardId);
}
