using KnowledgeBase.Application.DTOs;
using KnowledgeBase.Application.Interfaces;
using KnowledgeBase.Domain.Entities;
using KnowledgeBase.Domain.Interfaces;

namespace KnowledgeBase.Application.Services;

public class LexicalItemService : ILexicalItemService
{
    private readonly ILexicalItemRepository _repository;
    private readonly IGeminiService _gemini;

    public LexicalItemService(ILexicalItemRepository repository, IGeminiService gemini)
    {
        _repository = repository;
        _gemini = gemini;
    }

    public async Task<LookupResultDto> LookupAsync(string word)
    {
        // 1. Tìm trong kho trước
        var existing = await _repository.GetByValueAsync(word);
        if (existing != null)
        {
            return new LookupResultDto
            {
                Source = "vault",
                Id = existing.Id,
                Data = MapToAnalysisDto(existing)
            };
        }

        // 2. Chưa có → nhờ Gemini phân tích (chưa lưu)
        var analysis = await _gemini.AnalyzeWordAsync(word);
        return new LookupResultDto
        {
            Source = "generated",
            Id = null,
            Data = analysis
        };
    }

    public async Task<PagedResult<LexicalItem>> GetVaultAsync(string? topic, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var (items, total) = await _repository.GetPagedAsync(topic, page, pageSize);
        return new PagedResult<LexicalItem>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task<List<string>> GetTopicsAsync() => _repository.GetDistinctTopicsAsync();

    public Task<LexicalItem?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

    public async Task<LexicalItem> CreateAsync(CreateLexicalItemRequest request)
    {
        var item = new LexicalItem
        {
            Value = request.Value,
            Type = request.Type,
            Topics = request.Topics,
            Synonyms = request.Synonyms,
            Antonyms = request.Antonyms,
            PersonalNotes = request.PersonalNotes,
            Meanings = request.Meanings.Select(m => new Meaning
            {
                Definition = m.Definition,
                Connotation = m.Connotation,
                Examples = m.Examples
            }).ToList()
        };

        await _repository.CreateAsync(item);
        return item;
    }

    public async Task<bool> UpdateAsync(string id, UpdateLexicalItemRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        existing.Value = request.Value;
        existing.Type = request.Type;
        existing.Topics = request.Topics;
        existing.Synonyms = request.Synonyms;
        existing.Antonyms = request.Antonyms;
        existing.PersonalNotes = request.PersonalNotes;
        existing.Meanings = request.Meanings.Select(m => new Meaning
        {
            Definition = m.Definition,
            Connotation = m.Connotation,
            Examples = m.Examples
        }).ToList();

        await _repository.UpdateAsync(id, existing);
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        await _repository.DeleteAsync(id);
        return true;
    }

    private static WordAnalysisDto MapToAnalysisDto(LexicalItem item) => new()
    {
        Value = item.Value,
        Type = item.Type,
        Topics = item.Topics,
        Synonyms = item.Synonyms,
        Antonyms = item.Antonyms,
        Meanings = item.Meanings.Select(m => new MeaningDto
        {
            Definition = m.Definition,
            Connotation = m.Connotation,
            Examples = m.Examples
        }).ToList()
    };

    public Task<List<LexicalItem>> GetAllAsync() => _repository.GetAllAsync();

    public async Task<List<SuggestItemDto>> SearchAsync(string prefix, int limit)
    {
        var items = await _repository.SearchByPrefixAsync(prefix.Trim(), limit);
        return items.Select(i => new SuggestItemDto
        {
            Id = i.Id,
            Value = i.Value,
            Type = i.Type,
            Topics = i.Topics
        }).ToList();
    }
}