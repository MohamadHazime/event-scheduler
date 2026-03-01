using EventScheduler.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.ClientModel;

namespace EventScheduler.Infrastructure.Services;

public class GroqAiService : IAiService
{
    private readonly ChatClient _client;

    public GroqAiService(IConfiguration configuration)
    {
        var apiKey = configuration["Ai:ApiKey"]
            ?? throw new InvalidOperationException("Ai:ApiKey is not configured.");

        var options = new OpenAI.OpenAIClientOptions
        {
            Endpoint = new Uri("https://api.groq.com/openai/v1")
        };

        var client = new OpenAI.OpenAIClient(new ApiKeyCredential(apiKey), options);
        _client = client.GetChatClient("llama-3.3-70b-versatile");
    }

    public async Task<string> GenerateContentAsync(string prompt)
    {
        var completion = await _client.CompleteChatAsync(prompt);
        return completion.Value.Content[0].Text ?? string.Empty;
    }
}