// This following is the content of file: ClaudeChatCompletion.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace GlobalNameSpace;

public class ClaudeChatCompletion : IAiResponse
{
    public ChatMessage GetMessage() => new(Role, Content[0].Text);
    public string GetId() => Id;
    public string GetModel() => Model;
    public string GetRequestType() => Type;
    public string GetStopReason() => StopReason;
    public string GetTimeCreated() => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    public int GetInputTokens() => Usage.InputTokens;
    public int GetOutputTokens() => Usage.OutputTokens;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public ClaudeContent[] Content { get; set; } = [new()];

    [JsonPropertyName("stop_reason")]
    public string StopReason { get; set; } = string.Empty;

    [JsonPropertyName("stop_sequence")]
    public string? StopSequence { get; set; }

    [JsonPropertyName("usage")]
    public ClaudeUsage Usage { get; set; } = new();

    [JsonPropertyName("error")]
    public ClaudeErrorResponse? Error { get; set; }

    public class ClaudeContent
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    public class ClaudeUsage
    {
        [JsonPropertyName("input_tokens")]
        public int InputTokens { get; set; }

        [JsonPropertyName("output_tokens")]
        public int OutputTokens { get; set; }
    }

    public class ClaudeErrorResponse
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}