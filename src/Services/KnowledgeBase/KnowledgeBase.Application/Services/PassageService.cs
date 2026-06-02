using System.Collections.Generic;
using System.Threading.Tasks;
using KnowledgeBase.Application.DTOs;
using KnowledgeBase.Application.Interfaces;
using KnowledgeBase.Domain.Entities;
using KnowledgeBase.Domain.Interfaces;

namespace KnowledgeBase.Application.Services;

public class PassageService : IPassageService
{
    private readonly IGeneratedPassageRepository _passageRepository;
    private readonly ILexicalItemRepository _lexicalRepository;
    private readonly IGeminiService _geminiService;

    public PassageService(
        IGeneratedPassageRepository passageRepository,
        ILexicalItemRepository lexicalRepository,
        IGeminiService geminiService)
    {
        _passageRepository = passageRepository;
        _lexicalRepository = lexicalRepository;
        _geminiService = geminiService;
    }

    public async Task<GeneratedPassage> GenerateAndSavePassageAsync(GeneratePassageDto dto)
    {
        var words = new List<string>();
        foreach (var id in dto.LexicalItemIds)
        {
            var item = await _lexicalRepository.GetByIdAsync(id);
            if (item != null)
            {
                words.Add(item.Value);
            }
        }

        var (english, vietnamese) = await _geminiService.GenerateEssayAsync(dto.Topic, dto.TargetBand, words);

        var passage = new GeneratedPassage
        {
            Topic = dto.Topic,
            TargetBand = dto.TargetBand,
            EnglishContent = english,
            VietnameseTranslation = vietnamese,
            UsedLexicalItemIds = dto.LexicalItemIds
        };

        await _passageRepository.CreateAsync(passage);
        return passage;
    }

    public async Task<List<GeneratedPassage>> GetPassagesByTopicAsync(string topic)
    {
        return await _passageRepository.GetByTopicAsync(topic);
    }
}