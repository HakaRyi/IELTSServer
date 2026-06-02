using System.Collections.Generic;
using System.Threading.Tasks;

namespace KnowledgeBase.Application.Interfaces;

public interface IGeminiService
{
    Task<(string English, string Vietnamese)> GenerateEssayAsync(string topic, double band, List<string> words);
    Task<string> LookupWordAsync(string word);
    Task<WordAnalysisDto> AnalyzeWordAsync(string word);
}