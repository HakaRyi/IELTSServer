using KnowledgeBase.Application.DTOs;

namespace KnowledgeBase.Application.Interfaces;

/// <summary>Dịch vụ AI sinh văn bản (hiện dùng Groq). Provider-neutral.</summary>
public interface ILlmService
{
    Task<WordAnalysisDto> AnalyzeWordAsync(string word);
}
