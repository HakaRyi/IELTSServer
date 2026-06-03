using Practice.Domain.Entities;

namespace Practice.Domain.Interfaces;

public interface ISpeakingRepository
{
    Task<GeneratedSpeaking> CreateAsync(GeneratedSpeaking speaking);
    Task<List<GeneratedSpeaking>> GetByTopicAsync(string topic);
    Task<GeneratedSpeaking?> GetByIdAsync(string id);
}
