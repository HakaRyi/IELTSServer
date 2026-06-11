using Practice.Application.DTOs;
using Practice.Application.Interfaces;
using Practice.Domain.Entities;

namespace Practice.Application.Services;

public class PassageService : IPassageService
{
    private readonly IGeneratedPassageRepository _repository;
    private readonly ILexicalVaultClient _vaultClient;
    private readonly ILlmService _llm;

    public PassageService(
        IGeneratedPassageRepository repository,
        ILexicalVaultClient vaultClient,
        ILlmService llm)
    {
        _repository = repository;
        _vaultClient = vaultClient;
        _llm = llm;
    }

    public async Task<PassageResponse> GenerateAndSavePassageAsync(
        string userId, GeneratePassageRequest request)
    {
        var words = await _vaultClient.GetWordValuesByIdsAsync(userId, request.LexicalItemIds);

        var (english, vietnamese) = await _llm.GenerateEssayAsync(
            request.Topic, request.TargetBand, words);

        var passage = new GeneratedPassage
        {
            UserId = userId,
            Topic = request.Topic,
            TargetBand = request.TargetBand,
            EnglishContent = english,
            VietnameseTranslation = vietnamese,
            UsedLexicalItemIds = request.LexicalItemIds,
            UsedVocabulary = words
        };

        await _repository.CreateAsync(passage);
        return ToResponse(passage);
    }

    public async Task<List<PassageResponse>> GetPassagesByTopicAsync(string userId, string topic)
    {
        var passages = await _repository.GetByTopicAsync(userId, topic);
        return passages.Select(ToResponse).ToList();
    }

    public async Task<List<PassageResponse>> GetRecentAsync(string userId, int limit)
    {
        var passages = await _repository.GetRecentAsync(userId, limit);
        return passages.Select(ToResponse).ToList();
    }

    private static PassageResponse ToResponse(GeneratedPassage p) => new()
    {
        Id = p.Id,
        Topic = p.Topic,
        TargetBand = p.TargetBand,
        EnglishContent = p.EnglishContent,
        VietnameseTranslation = p.VietnameseTranslation,
        UsedLexicalItemIds = p.UsedLexicalItemIds,
        UsedVocabulary = p.UsedVocabulary,
        CreatedAt = p.CreatedAt
    };
}
