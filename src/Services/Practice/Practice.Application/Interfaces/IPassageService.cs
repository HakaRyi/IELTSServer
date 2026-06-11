using Practice.Application.DTOs;

namespace Practice.Application.Interfaces;

public interface IPassageService
{
    Task<PassageResponse> GenerateAndSavePassageAsync(string userId, GeneratePassageRequest request);
    Task<List<PassageResponse>> GetPassagesByTopicAsync(string userId, string topic);
    Task<List<PassageResponse>> GetRecentAsync(string userId, int limit);
}
