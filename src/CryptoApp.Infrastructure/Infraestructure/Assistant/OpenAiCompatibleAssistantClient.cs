using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CryptoAppBackEnd.Application.Ports;
using Microsoft.Extensions.Configuration;

namespace CryptoAppBackEnd.Infraestructure.Assistant
{
    /// <summary>
    /// Adaptador de salida hacia un endpoint compatible con la API de OpenAI
    /// (<c>POST {BaseUrl}/chat/completions</c>). Por defecto apunta a Ollama local
    /// (<c>http://localhost:11434/v1</c>, sin API key). Si la sección <c>Assistant:ApiKey</c>
    /// está configurada, añade <c>Authorization: Bearer</c> (Ollama lo ignora; OpenAI/otros lo usan).
    /// Lanza excepción ante error HTTP o respuesta vacía, para que el caso de uso caiga al fallback.
    /// </summary>
    public class OpenAiCompatibleAssistantClient : IAssistantProvider
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _model;
        private readonly string? _apiKey;

        public OpenAiCompatibleAssistantClient(HttpClient http, IConfiguration config)
        {
            _http = http;

            var baseUrl = config["Assistant:BaseUrl"];
            _baseUrl = string.IsNullOrWhiteSpace(baseUrl)
                ? "http://localhost:11434/v1"
                : baseUrl.TrimEnd('/');

            var model = config["Assistant:Model"];
            _model = string.IsNullOrWhiteSpace(model) ? "llama3.2" : model;

            var apiKey = config["Assistant:ApiKey"];
            _apiKey = string.IsNullOrWhiteSpace(apiKey) ? null : apiKey;
        }

        public async Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken ct = default)
        {
            var payload = new ChatRequest
            {
                Model = _model,
                Stream = false,
                Messages = new[]
                {
                    new ChatMessage { Role = "system", Content = systemPrompt },
                    new ChatMessage { Role = "user", Content = userPrompt }
                }
            };

            var json = JsonSerializer.Serialize(payload, JsonOptions);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/chat/completions")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (_apiKey is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            }

            var response = await _http.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"El proveedor de IA respondió {(int)response.StatusCode}.");
            }

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            var parsed = await JsonSerializer.DeserializeAsync<ChatResponse>(stream, JsonOptions, ct);

            var content = parsed?.Choices is { Count: > 0 }
                ? parsed.Choices[0].Message?.Content
                : null;

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException("El proveedor de IA devolvió una respuesta vacía.");
            }

            return content;
        }

        // --- DTOs del protocolo OpenAI chat/completions --------------------------------------------

        private sealed class ChatRequest
        {
            [JsonPropertyName("model")] public string Model { get; set; } = string.Empty;
            [JsonPropertyName("messages")] public ChatMessage[] Messages { get; set; } = Array.Empty<ChatMessage>();
            [JsonPropertyName("stream")] public bool Stream { get; set; }
        }

        private sealed class ChatMessage
        {
            [JsonPropertyName("role")] public string Role { get; set; } = string.Empty;
            [JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
        }

        private sealed class ChatResponse
        {
            [JsonPropertyName("choices")] public List<ChatChoice>? Choices { get; set; }
        }

        private sealed class ChatChoice
        {
            [JsonPropertyName("message")] public ChatMessage? Message { get; set; }
        }
    }
}
