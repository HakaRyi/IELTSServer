using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KnowledgeBase.Application.DTOs;
using KnowledgeBase.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KnowledgeBase.Infrastructure.ExternalServices;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
    }

    public async Task<(string English, string Vietnamese)> GenerateEssayAsync(string topic, double band, List<string> words)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={_apiKey}";
        var wordList = string.Join(", ", words);
        var prompt = $"Write an informative and engaging article with a minimum of 250 words about fascinating real-world events, historical facts, or scientific discoveries related to the topic: '{topic}'. The writing style should be academic and match an IELTS reading passage at a band score level of {band}. Ensure the text is properly formatted with clear paragraph breaks (use \\n\\n for new paragraphs). You MUST naturally integrate and highlight the following vocabulary words or phrases: {wordList}. Provide the output strictly as a clean JSON object with exactly two properties: 'english' (the full text with paragraph breaks) and 'vietnamese' (the accurate Vietnamese translation, matching the paragraph structure). Do not wrap the JSON response inside markdown blocks.";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonResponse);
        var textResult = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;

        var cleanJson = textResult.Replace("```json", "").Replace("```", "").Trim();
        using var resultDoc = JsonDocument.Parse(cleanJson);
        var english = resultDoc.RootElement.GetProperty("english").GetString() ?? string.Empty;
        var vietnamese = resultDoc.RootElement.GetProperty("vietnamese").GetString() ?? string.Empty;

        return (english, vietnamese);
    }

    public async Task<string> LookupWordAsync(string word)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={_apiKey}";
        var prompt = $"Analyze the English word \"{word}\" for IELTS learners. Return ONLY a JSON object with this exact structure: {{\"value\": \"...\", \"type\": \"noun|verb|adjective|adverb|...\", \"topics\": [\"topic1\", \"topic2\"], \"meanings\": [{{\"definition\": \"...(Vietnamese translation + English def)\", \"connotation\": \"Neutral|Positive|Negative|Formal|Informal\", \"examples\": [\"sentence1\", \"sentence2\"]}}], \"synonyms\": [\"...\"], \"antonyms\": [\"...\"]}}. Do not wrap the JSON response inside markdown blocks.";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Gemini API Error ({response.StatusCode}): {errorContent}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonResponse);
        var textResult = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;

        return textResult.Replace("```json", "").Replace("```", "").Trim();
    }

    public async Task<WordAnalysisDto> AnalyzeWordAsync(string word)
{
    var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={_apiKey}";

    var prompt = $@"Analyze the English word ""{word}"" for an IELTS learner.
Return STRICTLY a clean JSON object (no markdown) with this exact shape:
{{
  ""value"": ""the base form of the word"",
  ""type"": ""noun | verb | adjective | adverb | phrase"",
  ""topics"": [""2-4 IELTS topics this word commonly belongs to, e.g. Environment, Technology""],
  ""meanings"": [
    {{
      ""definition"": ""a clear definition in English, followed by ' — ' and the Vietnamese translation"",
      ""connotation"": ""Neutral | Positive | Negative | Formal | Informal"",
      ""examples"": [""one natural example sentence""]
    }}
  ],
  ""synonyms"": [""up to 4 synonyms""],
  ""antonyms"": [""up to 4 antonyms""]
}}
If the word has different meanings across topics, include multiple objects in 'meanings'.";

    var requestBody = new
    {
        contents = new[] { new { parts = new[] { new { text = prompt } } } }
    };

    var jsonRequest = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
    var response = await _httpClient.PostAsync(url, content);
    response.EnsureSuccessStatusCode();

    var jsonResponse = await response.Content.ReadAsStringAsync();
    using var doc = JsonDocument.Parse(jsonResponse);
    var textResult = doc.RootElement
        .GetProperty("candidates")[0]
        .GetProperty("content")
        .GetProperty("parts")[0]
        .GetProperty("text")
        .GetString() ?? string.Empty;

    var cleanJson = textResult.Replace("```json", "").Replace("```", "").Trim();

    var result = JsonSerializer.Deserialize<WordAnalysisDto>(cleanJson, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

    return result ?? throw new InvalidOperationException("Gemini returned invalid data.");
}
}