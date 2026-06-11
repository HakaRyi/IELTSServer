using Practice.Domain.Entities;

namespace Practice.Application.Interfaces;

public interface IGeneratedPassageRepository
{
    Task<GeneratedPassage> CreateAsync(GeneratedPassage passage);
    Task<List<GeneratedPassage>> GetByTopicAsync(string userId, string topic);
    Task<GeneratedPassage?> GetByIdAsync(string userId, string id);
    Task<List<GeneratedPassage>> GetRecentAsync(string userId, int limit);
}
