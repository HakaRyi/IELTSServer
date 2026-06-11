using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using KnowledgeBase.Application.DTOs;
using KnowledgeBase.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KnowledgeBase.Infrastructure.ExternalServices;

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

    public async Task<WordAnalysisDto> AnalyzeWordAsync(string word)
    {
        var prompt = $@"Analyze the English word ""{word}"" for an IELTS learner.
Return a JSON object with this exact shape:
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

        var json = await CallGroqAsync(prompt);

        var result = JsonSerializer.Deserialize<WordAnalysisDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result ?? throw new InvalidOperationException("Groq returned invalid data.");
    }

    /// <summary>Gọi Groq chat completions với JSON mode, trả về chuỗi JSON sạch.</summary>
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
