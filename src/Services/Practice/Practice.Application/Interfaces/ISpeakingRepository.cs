using Practice.Domain.Entities;

namespace Practice.Application.Interfaces;

public interface ISpeakingRepository
{
    Task<GeneratedSpeaking> CreateAsync(GeneratedSpeaking speaking);
    Task<List<GeneratedSpeaking>> GetByTopicAsync(string topic);
    Task<GeneratedSpeaking?> GetByIdAsync(string id);
    Task<List<GeneratedSpeaking>> GetRecentAsync(int limit);
}
