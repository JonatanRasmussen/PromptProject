namespace GlobalNameSpace;

public static class Anthropic
{
    private class Claude3ApiAccess : IApiAccess
    {
        public string Endpoint { get; } = "https://api.anthropic.com/v1/messages";
        public string ApiKey { get; } = MyLocalConfigs.ApiKeyAnthropic;
        public ChatRoles ChatRoles { get; } = new("user", "assistant", "system");
        public string AnthropicVersion { get; } = "2023-06-01";
        public IAiResponse RequestChatCompletion(AiRequest request)
        {
            List<Tuple<string, string>> headerData =
            [
                new ("x-api-key", ApiKey),
                new ("anthropic-version", AnthropicVersion)
            ];
            return ApiAccessUtils.CallAPI<ClaudeChatCompletion>(Endpoint, headerData, request);
        }
    }

    public class Claude3Opus : IAiModel
    {
        public string ModelName { get; } = "claude-3-opus-20240229";
        public string DisplayName { get; } = "Claude-3 Opus";
        public IApiAccess ApiAccess { get; } = new Claude3ApiAccess();
        public int ContextWindow { get; } = 200000;
        public int MaxOutputTokens { get; } = 4096;
        public double InputPricePerMTokensInUSD { get; } = 15.00;
        public double OutputPricePerMTokensInUSD { get; } = 75.00;
        public DateTime TrainingCutoff { get; } = new(2023, 8, 1);
    }

    public class Claude3Sonnet : IAiModel
    {
        public string ModelName { get; } = "claude-3-sonnet-20240229";
        public string DisplayName { get; } = "Claude-3 Sonnet";
        public IApiAccess ApiAccess { get; } = new Claude3ApiAccess();
        public int ContextWindow { get; } = 200000;
        public int MaxOutputTokens { get; } = 4096;
        public double InputPricePerMTokensInUSD { get; } = 3.00;
        public double OutputPricePerMTokensInUSD { get; } = 15.00;
        public DateTime TrainingCutoff { get; } = new(2023, 8, 1);
    }

    public class Claude3Haiku : IAiModel
    {
        public string ModelName { get; } = "claude-3-haiku-20240307";
        public string DisplayName { get; } = "Claude 3 Haiku";
        public IApiAccess ApiAccess { get; } = new Claude3ApiAccess();
        public int ContextWindow { get; } = 200000;
        public int MaxOutputTokens { get; } = 4096;
        public double InputPricePerMTokensInUSD { get; } = 0.25;
        public double OutputPricePerMTokensInUSD { get; } = 1.25;
        public DateTime TrainingCutoff { get; } = new(2023, 8, 1);
    }
}