using Practice.Application.DTOs;

namespace Practice.Application.Interfaces;

public interface IGeminiService
{
    Task<(string English, string Vietnamese)> GenerateEssayAsync(string topic, double band, List<string> words);
    Task<SpeakingGeminiResult> GenerateSpeakingAsync(string topic, double band, List<string> words);
}
