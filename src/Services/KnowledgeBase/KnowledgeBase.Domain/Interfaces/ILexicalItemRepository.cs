using KnowledgeBase.Domain.Entities;

namespace KnowledgeBase.Domain.Interfaces;

public interface ILexicalItemRepository
{
    Task<List<LexicalItem>> GetAllAsync();
    Task<LexicalItem?> GetByIdAsync(string id);
    Task CreateAsync(LexicalItem item);
    Task UpdateAsync(string id, LexicalItem item);
    Task DeleteAsync(string id);

    Task<LexicalItem?> GetByValueAsync(string value);
    Task<(List<LexicalItem> Items, long Total)> GetPagedAsync(string? topic, int page, int pageSize);
    Task<List<string>> GetDistinctTopicsAsync();
    Task<List<LexicalItem>> SearchByPrefixAsync(string prefix, int limit);
}