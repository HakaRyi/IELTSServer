using Practice.Application.DTOs;
using Practice.Application.Interfaces;
using Practice.Domain.Entities;

namespace Practice.Application.Services;

public class EssayService : IEssayService
{
    private readonly IEssayRepository _repository;
    private readonly ILexicalVaultClient _vaultClient;
    private readonly ILlmService _llm;

    public EssayService(
        IEssayRepository repository,
        ILexicalVaultClient vaultClient,
        ILlmService llm)
    {
        _repository = repository;
        _vaultClient = vaultClient;
        _llm = llm;
    }

    public async Task<EssayResponse> ScoreAndSaveAsync(string userId, ScoreEssayRequest request)
    {
        // Lấy toàn bộ từ vựng user đã học (mọi topic) để chấm tiêu chí "dùng từ đã học"
        var learnedWords = await _vaultClient.GetWordValuesByTopicAsync(userId, string.Empty);

        var score = await _llm.ScoreEssayAsync(request.Prompt, request.EssayText, learnedWords);

        var wordCount = request.EssayText
            .Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Length;

        var essay = new GeneratedEssay
        {
            UserId = userId,
            Prompt = request.Prompt,
            EssayText = request.EssayText,
            WordCount = wordCount,
            OverallBand = score.OverallBand,
            TaskResponse = Map(score.TaskResponse),
            CoherenceCohesion = Map(score.CoherenceCohesion),
            LexicalResource = Map(score.LexicalResource),
            GrammaticalRange = Map(score.GrammaticalRange),
            GeneralFeedback = score.GeneralFeedback,
            Improvements = score.Improvements,
            UsedTargetVocabulary = score.UsedTargetVocabulary
        };

        await _repository.CreateAsync(essay);
        return ToResponse(essay);
    }

    public async Task<List<EssayResponse>> GetRecentAsync(string userId, int limit)
    {
        var list = await _repository.GetRecentAsync(userId, limit);
        return list.Select(ToResponse).ToList();
    }

    private static CriterionScore Map(GroqCriterion c)
        => new() { Band = c.Band, Comment = c.Comment };

    private static CriterionScoreDto MapDto(CriterionScore c)
        => new() { Band = c.Band, Comment = c.Comment };

    private static EssayResponse ToResponse(GeneratedEssay e) => new()
    {
        Id = e.Id,
        Prompt = e.Prompt,
        EssayText = e.EssayText,
        WordCount = e.WordCount,
        OverallBand = e.OverallBand,
        TaskResponse = MapDto(e.TaskResponse),
        CoherenceCohesion = MapDto(e.CoherenceCohesion),
        LexicalResource = MapDto(e.LexicalResource),
        GrammaticalRange = MapDto(e.GrammaticalRange),
        GeneralFeedback = e.GeneralFeedback,
        Improvements = e.Improvements,
        UsedTargetVocabulary = e.UsedTargetVocabulary,
        CreatedAt = e.CreatedAt
    };
}
