using Practice.Application.DTOs;

namespace Practice.Application.Interfaces;

public interface ISpeakingService
{
    Task<SpeakingResponse> GenerateAndSaveAsync(GenerateSpeakingRequest request);
    Task<List<SpeakingResponse>> GetByTopicAsync(string topic);
    Task<List<SpeakingResponse>> GetRecentAsync(int limit);
}
