using Practice.Domain.Entities;

namespace Practice.Application.Interfaces;

public interface IGeneratedPassageRepository
{
    Task<GeneratedPassage> CreateAsync(GeneratedPassage passage);
    Task<List<GeneratedPassage>> GetByTopicAsync(string topic);
    Task<GeneratedPassage?> GetByIdAsync(string id);
    Task<List<GeneratedPassage>> GetRecentAsync(int limit);
}
