using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Practice.Application.DTOs;
using Practice.Application.Interfaces;

namespace Practice.Infrastructure.ExternalServices;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl =
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
    }

    // ─── Reading passage ─────────────────────────────────────────────────────

    public async Task<(string English, string Vietnamese)> GenerateEssayAsync(
        string topic, double band, List<string> words)
    {
        var wordList = string.Join(", ", words);
        var prompt = $@"Write an informative and engaging article with a minimum of 250 words about the topic: '{topic}'.
The writing style should be academic and match an IELTS reading passage at band score {band}.
Use clear paragraph breaks (\n\n). Naturally integrate these vocabulary words: {wordList}.
Return ONLY a clean JSON object (no markdown) with exactly two properties:
""english"" (full text) and ""vietnamese"" (accurate Vietnamese translation, same paragraph structure).";

        var text = await CallGeminiAsync(prompt);
        using var doc = JsonDocument.Parse(text);
        return (
            doc.RootElement.GetProperty("english").GetString() ?? string.Empty,
            doc.RootElement.GetProperty("vietnamese").GetString() ?? string.Empty
        );
    }

    // ─── Speaking practice ───────────────────────────────────────────────────

    public async Task<SpeakingGeminiResult> GenerateSpeakingAsync(
        string topic, double band, List<string> words)
    {
        var wordList = words.Count > 0
            ? $"You MUST naturally use these vocabulary words in the sample answers: {string.Join(", ", words)}."
            : string.Empty;

        var prompt = $@"Generate a complete IELTS Speaking practice set for the topic: ""{topic}"".
Target band score: {band}. {wordList}

Return ONLY a clean JSON object (no markdown) with this exact structure:
{{
  ""part1"": [
    {{ ""question"": ""..."", ""sampleAnswer"": ""..."" }},
    {{ ""question"": ""..."", ""sampleAnswer"": ""..."" }},
    {{ ""question"": ""..."", ""sampleAnswer"": ""..."" }}
  ],
  ""part2"": {{
    ""cueCard"": ""Describe ..."",
    ""points"": [""What/Who/When/Where..."", ""..."", ""...""],
    ""sampleAnswer"": ""A 1.5-2 minute model answer (at least 200 words).""
  }},
  ""part3"": [
    {{ ""question"": ""..."", ""sampleAnswer"": ""..."" }},
    {{ ""question"": ""..."", ""sampleAnswer"": ""..."" }},
    {{ ""question"": ""..."", ""sampleAnswer"": ""..."" }}
  ],
  ""usedVocabulary"": [""word1"", ""word2"", ...]
}}

Rules:
- Part 1: 3 short personal questions with concise answers (2-4 sentences).
- Part 2: a cue card with 3-4 bullet points and a detailed 200+ word model answer.
- Part 3: 3 abstract/discussion questions with thorough answers (4-6 sentences each).
- Sample answers must sound natural and match band {band} proficiency.";

        var text = await CallGeminiAsync(prompt);
        var result = JsonSerializer.Deserialize<SpeakingGeminiResult>(text,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return result ?? throw new InvalidOperationException("Gemini returned invalid speaking data.");
    }

    // ─── Shared helper ───────────────────────────────────────────────────────

    private async Task<string> CallGeminiAsync(string prompt)
    {
        var url = $"{BaseUrl}?key={_apiKey}";
        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonResponse);
        var raw = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;

        return raw.Replace("```json", "").Replace("```", "").Trim();
    }
}
