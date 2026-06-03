using Practice.Application.DTOs;
using Practice.Application.Interfaces;
using Practice.Domain.Entities;

namespace Practice.Application.Services;

public class SpeakingService : ISpeakingService
{
    private readonly ISpeakingRepository _repository;
    private readonly ILexicalVaultClient _vaultClient;
    private readonly IGeminiService _geminiService;

    public SpeakingService(
        ISpeakingRepository repository,
        ILexicalVaultClient vaultClient,
        IGeminiService geminiService)
    {
        _repository = repository;
        _vaultClient = vaultClient;
        _geminiService = geminiService;
    }

    public async Task<SpeakingResponse> GenerateAndSaveAsync(GenerateSpeakingRequest request)
    {
        // Nếu client truyền danh sách từ rỗng thì lấy từ vault theo chủ đề (tất cả)
        var words = request.VocabularyWords.Count > 0
            ? request.VocabularyWords
            : await _vaultClient.GetWordValuesByTopicAsync(request.Topic);

        var geminiResult = await _geminiService.GenerateSpeakingAsync(
            request.Topic, request.TargetBand, words);

        var entity = new GeneratedSpeaking
        {
            Topic = request.Topic,
            TargetBand = request.TargetBand,
            Part1 = geminiResult.Part1.Select(q => new SpeakingQA
                { Question = q.Question, SampleAnswer = q.SampleAnswer }).ToList(),
            Part2 = new SpeakingPart2
            {
                CueCard = geminiResult.Part2.CueCard,
                Points = geminiResult.Part2.Points,
                SampleAnswer = geminiResult.Part2.SampleAnswer
            },
            Part3 = geminiResult.Part3.Select(q => new SpeakingQA
                { Question = q.Question, SampleAnswer = q.SampleAnswer }).ToList(),
            UsedVocabulary = geminiResult.UsedVocabulary
        };

        await _repository.CreateAsync(entity);
        return ToResponse(entity);
    }

    public async Task<List<SpeakingResponse>> GetByTopicAsync(string topic)
    {
        var list = await _repository.GetByTopicAsync(topic);
        return list.Select(ToResponse).ToList();
    }

    public async Task<List<SpeakingResponse>> GetRecentAsync(int limit)
    {
        var list = await _repository.GetRecentAsync(limit);
        return list.Select(ToResponse).ToList();
    }

    private static SpeakingResponse ToResponse(GeneratedSpeaking e) => new()
    {
        Id = e.Id,
        Topic = e.Topic,
        TargetBand = e.TargetBand,
        Part1 = e.Part1.Select(q => new SpeakingQADto
            { Question = q.Question, SampleAnswer = q.SampleAnswer }).ToList(),
        Part2 = new SpeakingPart2Dto
        {
            CueCard = e.Part2.CueCard,
            Points = e.Part2.Points,
            SampleAnswer = e.Part2.SampleAnswer
        },
        Part3 = e.Part3.Select(q => new SpeakingQADto
            { Question = q.Question, SampleAnswer = q.SampleAnswer }).ToList(),
        UsedVocabulary = e.UsedVocabulary,
        CreatedAt = e.CreatedAt
    };
}
