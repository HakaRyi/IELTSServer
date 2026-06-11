using Practice.Domain.Entities;

namespace Practice.Application.Interfaces;

public interface IEssayRepository
{
    Task<GeneratedEssay> CreateAsync(GeneratedEssay essay);
    Task<GeneratedEssay?> GetByIdAsync(string userId, string id);
    Task<List<GeneratedEssay>> GetRecentAsync(string userId, int limit);
}
