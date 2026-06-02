using KnowledgeBase.Application.DTOs;
using KnowledgeBase.Domain.Entities;

namespace KnowledgeBase.Application.Interfaces;

public interface ILexicalItemService
{
    Task<LookupResultDto> LookupAsync(string word);
    Task<PagedResult<LexicalItem>> GetVaultAsync(string? topic, int page, int pageSize);
    Task<List<string>> GetTopicsAsync();
    Task<LexicalItem?> GetByIdAsync(string id);
    Task<LexicalItem> CreateAsync(CreateLexicalItemRequest request);
    Task<bool> UpdateAsync(string id, UpdateLexicalItemRequest request);
    Task<bool> DeleteAsync(string id);
    Task<List<LexicalItem>> GetAllAsync();
}