using KnowledgeBase.Domain.Entities;

namespace KnowledgeBase.Domain.Interfaces;

public interface ILexicalItemRepository
{
    Task<List<LexicalItem>> GetAllAsync(string userId);
    Task<LexicalItem?> GetByIdAsync(string userId, string id);
    Task CreateAsync(LexicalItem item);
    Task UpdateAsync(string id, LexicalItem item);
    Task DeleteAsync(string userId, string id);

    Task<LexicalItem?> GetByValueAsync(string userId, string value);
    Task<(List<LexicalItem> Items, long Total)> GetPagedAsync(string userId, string? topic, int page, int pageSize);
    Task<List<string>> GetDistinctTopicsAsync(string userId);
    Task<List<LexicalItem>> SearchByPrefixAsync(string userId, string prefix, int limit);
    Task<List<(string Topic, long Count)>> GetTopicStatsAsync(string userId);
}
