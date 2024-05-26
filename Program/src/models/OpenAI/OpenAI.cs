namespace GlobalNameSpace;

public static class OpenAI
{
    public static async Task RequestChatCompletionStream(AiRequest request)
    {
        ChatGPTApiAccess apiAccess = new();
        await apiAccess.RequestChatCompletionStream(request);
    }

    private class ChatGPTApiAccess : IApiAccess
    {
        // https://platform.openai.com/docs/api-reference/chat
        public string Endpoint { get; } = "https://api.openai.com/v1/chat/completions";
        public string ApiKey { get; } = MyLocalConfigs.ApiKeyOpenAI;
        public ChatRoles ChatRoles { get; } = new("user", "assistant", "system");

        public IAiResponse RequestChatCompletion(AiRequest request)
        {
            List<Tuple<string, string>> headerData =
            [
                new ("Authorization", "Bearer " + ApiKey),
            ];
            return ApiAccessUtils.CallAPI<ChatGPTChatCompletion>(Endpoint, headerData, request);
        }

        public async Task RequestChatCompletionStream(AiRequest request)
        {
            List<Tuple<string, string>> headerData =
            [
                new ("Authorization", "Bearer " + ApiKey),
            ];
            await ApiAccessUtils.StreamAPIAsync(Endpoint, headerData, request);
        }
    }

    public class Gpt4o : IAiModel
    {
        public string ModelName { get; } = "gpt-4o";
        public string DisplayName { get; } = "ChatGPT-4o";
        public IApiAccess ApiAccess { get; } = new ChatGPTApiAccess();
        public int ContextWindow { get; } = 128000;
        public int MaxOutputTokens { get; } = 4096;
        public double InputPricePerMTokensInUSD { get; } = 5.00;
        public double OutputPricePerMTokensInUSD { get; } = 15.00;
        public DateTime TrainingCutoff { get; } = new DateTime(2023, 12, 1);
    }

    public class Gpt4Turbo : IAiModel
    {
        public string ModelName { get; } = "gpt-4-turbo";
        public string DisplayName { get; } = "ChatGPT-4 Turbo";
        public IApiAccess ApiAccess { get; } = new ChatGPTApiAccess();
        public int ContextWindow { get; } = 128000;
        public int MaxOutputTokens { get; } = 4096;
        public double InputPricePerMTokensInUSD { get; } = 10.00;
        public double OutputPricePerMTokensInUSD { get; } = 30.00;
        public DateTime TrainingCutoff { get; } = new DateTime(2023, 12, 1);
    }

    public class Gpt35Turbo : IAiModel
    {
        public string ModelName { get; } = "gpt-3.5-turbo";
        public string DisplayName { get; } = "ChatGPT-3.5 Turbo";
        public IApiAccess ApiAccess { get; } = new ChatGPTApiAccess();
        public int ContextWindow { get; } = 16385;
        public int MaxOutputTokens { get; } = 4096;
        public double InputPricePerMTokensInUSD { get; } = 0.50;
        public double OutputPricePerMTokensInUSD { get; } = 1.50;
        public DateTime TrainingCutoff { get; } = new DateTime(2021, 9, 1);
    }
}