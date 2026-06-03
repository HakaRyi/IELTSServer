using Practice.Application.DTOs;

namespace Practice.Application.Interfaces;

public interface IPassageService
{
    Task<PassageResponse> GenerateAndSavePassageAsync(GeneratePassageRequest request);
    Task<List<PassageResponse>> GetPassagesByTopicAsync(string topic);
    Task<List<PassageResponse>> GetRecentAsync(int limit);
}
