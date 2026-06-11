using Practice.Application.DTOs;

namespace Practice.Application.Interfaces;

/// <summary>Dịch vụ AI sinh văn bản (hiện dùng Groq). Provider-neutral.</summary>
public interface ILlmService
{
    Task<(string English, string Vietnamese)> GenerateEssayAsync(string topic, double band, List<string> words);
    Task<SpeakingGeminiResult> GenerateSpeakingAsync(string topic, double band, List<string> words);
}
