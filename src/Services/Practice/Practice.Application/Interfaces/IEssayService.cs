using Practice.Application.DTOs;

namespace Practice.Application.Interfaces;

public interface IEssayService
{
    Task<EssayResponse> ScoreAndSaveAsync(string userId, ScoreEssayRequest request);
    Task<List<EssayResponse>> GetRecentAsync(string userId, int limit);
}
