using KnowledgeBase.Application.DTOs;
using KnowledgeBase.Domain.Entities;

namespace KnowledgeBase.Application.Interfaces;

public interface ILexicalItemService
{
    Task<LookupResultDto> LookupAsync(string userId, string word);
    Task<PagedResult<LexicalItem>> GetVaultAsync(string userId, string? topic, int page, int pageSize);
    Task<List<string>> GetTopicsAsync(string userId);
    Task<LexicalItem?> GetByIdAsync(string userId, string id);
    Task<LexicalItem> CreateAsync(string userId, CreateLexicalItemRequest request);
    Task<bool> UpdateAsync(string userId, string id, UpdateLexicalItemRequest request);
    Task<bool> DeleteAsync(string userId, string id);
    Task<List<LexicalItem>> GetAllAsync(string userId);
    Task<List<SuggestItemDto>> SearchAsync(string userId, string prefix, int limit);
}
