using Practice.Domain.Entities;

namespace Practice.Domain.Interfaces;

public interface IGeneratedPassageRepository
{
    Task<GeneratedPassage> CreateAsync(GeneratedPassage passage);
    Task<List<GeneratedPassage>> GetByTopicAsync(string topic);
    Task<GeneratedPassage?> GetByIdAsync(string id);
}
