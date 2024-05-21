using System.Text.Json.Serialization;
namespace GlobalNameSpace;


public class ChatGPTChatCompletion : IAiResponse
{
    public ChatMessage GetMessage() => new(Choices[0].Message.Role, Choices[0].Message.Content);
    public string GetId() => Id;
    public string GetModel() => Model;
    public string GetRequestType() => Object;
    public string GetStopReason() => Choices[0].FinishReason;
    public string GetTimeCreated() => Created.ToString();
    public int GetInputTokens() => Usage.PromptTokens;
    public int GetOutputTokens() => Usage.CompletionTokens;

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
    public ChatGPTChatCompletionChoice[] Choices { get; set; } = [new()];

    [JsonPropertyName("usage")]
    public ChatGPTChatCompletionUsage Usage { get; set; } = new();

    public class ChatGPTChatCompletionChoice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public ChatGPTChatCompletionMessage Message { get; set; } = new();

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = string.Empty;

        [JsonPropertyName("logprobs")]
        public object? Logprobs { get; set; } // Can be null or an object
    }

    public class ChatGPTChatCompletionMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    public class ChatGPTChatCompletionUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}