using Practice.Application.DTOs;

namespace Practice.Application.Interfaces;

public interface ISpeakingService
{
    Task<SpeakingResponse> GenerateAndSaveAsync(string userId, GenerateSpeakingRequest request);
    Task<List<SpeakingResponse>> GetByTopicAsync(string userId, string topic);
    Task<List<SpeakingResponse>> GetRecentAsync(string userId, int limit);
}
