using Practice.Application.DTOs;
using Practice.Application.Interfaces;
using Practice.Domain.Entities;

namespace Practice.Application.Services;

public class PassageService : IPassageService
{
    private readonly IGeneratedPassageRepository _repository;
    private readonly ILexicalVaultClient _vaultClient;
    private readonly IGeminiService _geminiService;

    public PassageService(
        IGeneratedPassageRepository repository,
        ILexicalVaultClient vaultClient,
        IGeminiService geminiService)
    {
        _repository = repository;
        _vaultClient = vaultClient;
        _geminiService = geminiService;
    }

    public async Task<PassageResponse> GenerateAndSavePassageAsync(GeneratePassageRequest request)
    {
        var words = await _vaultClient.GetWordValuesByIdsAsync(request.LexicalItemIds);

        var (english, vietnamese) = await _geminiService.GenerateEssayAsync(
            request.Topic, request.TargetBand, words);

        var passage = new GeneratedPassage
        {
            Topic = request.Topic,
            TargetBand = request.TargetBand,
            EnglishContent = english,
            VietnameseTranslation = vietnamese,
            UsedLexicalItemIds = request.LexicalItemIds
        };

        await _repository.CreateAsync(passage);
        return ToResponse(passage);
    }

    public async Task<List<PassageResponse>> GetPassagesByTopicAsync(string topic)
    {
        var passages = await _repository.GetByTopicAsync(topic);
        return passages.Select(ToResponse).ToList();
    }

    public async Task<List<PassageResponse>> GetRecentAsync(int limit)
    {
        var passages = await _repository.GetRecentAsync(limit);
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
        CreatedAt = p.CreatedAt
    };
}
