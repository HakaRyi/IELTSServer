using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Practice.Application.DTOs;
using Practice.Application.Interfaces;

namespace Practice.Infrastructure.ExternalServices;

/// <summary>Groq (OpenAI-compatible) — JSON mode ép model trả JSON hợp lệ.</summary>
public class GroqService : ILlmService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private const string Url = "https://api.groq.com/openai/v1/chat/completions";

    public GroqService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Groq:ApiKey"] ?? string.Empty;
        _model = configuration["Groq:Model"] ?? "llama-3.3-70b-versatile";
    }

    // ─── Reading passage ─────────────────────────────────────────────────────

    public async Task<(string English, string Vietnamese)> GenerateEssayAsync(
        string topic, double band, List<string> words)
    {
        var wordList = string.Join(", ", words);
        var prompt = $@"Write an informative and engaging article with a minimum of 250 words about the topic: '{topic}'.
The writing style should be academic and match an IELTS reading passage at band score {band}.
Use clear paragraph breaks (\n\n). Naturally integrate these vocabulary words: {wordList}.
Return a JSON object with exactly two properties:
""english"" (full text) and ""vietnamese"" (accurate Vietnamese translation, same paragraph structure).";

        var json = await CallGroqAsync(prompt);
        using var doc = JsonDocument.Parse(json);
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

Return a JSON object with this exact structure:
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
  ""usedVocabulary"": [""word1"", ""word2""]
}}

Rules:
- Part 1: 3 short personal questions with concise answers (2-4 sentences).
- Part 2: a cue card with 3-4 bullet points and a detailed 200+ word model answer.
- Part 3: 3 abstract/discussion questions with thorough answers (4-6 sentences each).
- Sample answers must sound natural and match band {band} proficiency.";

        var json = await CallGroqAsync(prompt);
        var result = JsonSerializer.Deserialize<SpeakingGeminiResult>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return result ?? throw new InvalidOperationException("Groq returned invalid speaking data.");
    }

    // ─── Essay scoring ───────────────────────────────────────────────────────

    public async Task<EssayScoreResult> ScoreEssayAsync(
        string prompt, string essayText, List<string> targetWords)
    {
        var vocabNote = targetWords.Count > 0
            ? $@"The learner has been studying these vocabulary words: {string.Join(", ", targetWords)}.
In ""usedTargetVocabulary"" list ONLY the studied words that the learner actually used correctly and naturally in the essay (empty list if none)."
            : @"In ""usedTargetVocabulary"" return an empty list.";

        var fullPrompt = $@"You are a certified IELTS Writing examiner. Score the following IELTS Writing Task 2 essay
using the official 4 band-descriptor criteria. Be strict and realistic (band 0-9, allow .5 steps).

PROMPT (the essay question):
{prompt}

ESSAY (the candidate's answer):
{essayText}

{vocabNote}

Return a JSON object with this exact structure:
{{
  ""overallBand"": 6.5,
  ""taskResponse"": {{ ""band"": 6.5, ""comment"": ""..."" }},
  ""coherenceCohesion"": {{ ""band"": 6.0, ""comment"": ""..."" }},
  ""lexicalResource"": {{ ""band"": 7.0, ""comment"": ""..."" }},
  ""grammaticalRange"": {{ ""band"": 6.0, ""comment"": ""..."" }},
  ""generalFeedback"": ""2-4 sentences overall summary"",
  ""improvements"": [""concrete actionable suggestion 1"", ""suggestion 2"", ""suggestion 3""],
  ""usedTargetVocabulary"": [""word1"", ""word2""]
}}

Rules:
- overallBand = average of the 4 criteria rounded to nearest 0.5.
- Comments must reference specific issues in THIS essay, not generic advice.
- Write all comments and feedback in English.";

        var json = await CallGroqAsync(fullPrompt);
        var result = JsonSerializer.Deserialize<EssayScoreResult>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return result ?? throw new InvalidOperationException("Groq returned invalid essay score.");
    }

    // ─── Shared helper ───────────────────────────────────────────────────────

    private async Task<string> CallGroqAsync(string prompt)
    {
        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = "You are an assistant that outputs only valid JSON." },
                new { role = "user", content = prompt }
            },
            response_format = new { type = "json_object" },
            temperature = 0.7
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, Url) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Groq API Error ({(int)response.StatusCode} {response.StatusCode}): {errorBody}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;
    }
}
