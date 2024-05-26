using System.Text.Json.Serialization;
namespace GlobalNameSpace;

public class ChatGPTChatCompletionChunk
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;

    [JsonPropertyName("created")]
    public int Created { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("system_fingerprint")]
    public string? SystemFingerprint { get; set; }

    [JsonPropertyName("choices")]
    public ChatGPTChatCompletionChoiceChunk[] Choices { get; set; } = [new()];

    public class ChatGPTChatCompletionChoiceChunk
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("delta")]
        public ChatGPTChatCompletionDelta Delta { get; set; } = new();

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }

        [JsonPropertyName("logprobs")]
        public object? Logprobs { get; set; } // Can be null or an object
    }

    public class ChatGPTChatCompletionDelta
    {
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}