using Practice.Domain.Entities;

namespace Practice.Application.Interfaces;

public interface ISpeakingRepository
{
    Task<GeneratedSpeaking> CreateAsync(GeneratedSpeaking speaking);
    Task<List<GeneratedSpeaking>> GetByTopicAsync(string userId, string topic);
    Task<GeneratedSpeaking?> GetByIdAsync(string userId, string id);
    Task<List<GeneratedSpeaking>> GetRecentAsync(string userId, int limit);
}
