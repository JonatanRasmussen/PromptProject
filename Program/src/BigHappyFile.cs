using System.Text;
using System.Text.Json;
namespace GlobalNameSpace;

public class PromptManager
{
    public static void ExecutePromptPipeline(IAiModel aiModel)
    {
        long promptNumber = GeneratePromptNumber();
        string prompt = LoadPrompt();
        SaveCopyOfPrompt(promptNumber, prompt);
        AiRequest request = PreparePrompt(aiModel, prompt);
        IAiResponse response = SubmitPrompt(aiModel, request);
        SaveResponse(promptNumber, response);
        PrintCost(aiModel, response);
    }

    private static long GeneratePromptNumber()
    {
        // Newer prompts should appear at the top when sorted alphabetically
        // This is because my VSCode config just so happens to be setup this way
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
        long epochTimeSeconds = currentTime.ToUnixTimeSeconds();
        long numberToCountDownFrom = 10_000_000_000;
        return numberToCountDownFrom - epochTimeSeconds;
    }

    private static string LoadPrompt()
    {
        return Utils.ReadFile(ProgramPaths.Prompt, ProgramFiles.Prompt);
    }

    private static void SaveCopyOfPrompt(long promptNumber, string prompt)
    {
        string nameAndExt = ProgramFiles.FormatInputName(promptNumber);
        Utils.WriteFile(ProgramPaths.Archive, nameAndExt, prompt);
    }

    private static AiRequest PreparePrompt(IAiModel aiModel, string prompt)
    {
        AiRequest aiRequest = new(aiModel);
        aiRequest.AddUserMessage(prompt);
        return aiRequest;
    }

    private static IAiResponse SubmitPrompt(IAiModel aiModel, AiRequest request)
    {
        try
        {
            IAiResponse response = aiModel.ApiAccess.RequestChatCompletion(request);
            //string response = aiResponse.GetMessage().Content;
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine("BudoError: " + ex.Message);
            return new AiErrorResponse(ex.Message);
        }
    }

    private static void SaveResponse(long promptNumber, IAiResponse response)
    {
        string completion = response.GetMessage().Content;
        string nameAndExt = ProgramFiles.FormatOutputName(promptNumber);
        Utils.WriteFile(ProgramPaths.Archive, nameAndExt, completion);
    }

    private static void PrintCost(IAiModel aiModel, IAiResponse response)
    {
        int inputTokens = response.GetInputTokens();
        int outputTokens = response.GetOutputTokens();
        string totalCost = PromptCostCalculator.GetCost(aiModel, inputTokens, outputTokens);
        Console.WriteLine(totalCost);
    }
}

public static class Utils
{
    public static string ReadFile(string filePath, string fileNameAndExt)
    {
        string file = Path.Combine(filePath, fileNameAndExt);
        using StreamReader reader = new(file);
        return reader.ReadToEnd();
    }

    public static void WriteFile(string filePath, string fileNameAndExt, string content)
    {
        string file = Path.Combine(filePath, fileNameAndExt);
        using StreamWriter writer = new(file);
        writer.Write(content);
    }
}

public class PromptCostCalculator
{
    private const double WordsPerToken = 0.75;  // Assuming 1 token = 0.75 words
    private const double USDToDKKExchangeRate = 6.91;
    private static readonly bool DisplayCostInDKK = true;

    public static string GetCost(IAiModel aiModel, int inputTokens, int outputTokens)
    {
        double inputCost = CalculateCost(inputTokens, aiModel.InputPricePerMTokensInUSD);
        double outputCost = CalculateCost(outputTokens, aiModel.OutputPricePerMTokensInUSD);
        return DisplayCost(inputCost, outputCost);
    }

    public static string EstimateCost(IAiModel aiModel, string prompt, string response)
    {
        double inputCost = EstimateCost(prompt, aiModel.InputPricePerMTokensInUSD);
        double outputCost = EstimateCost(response, aiModel.OutputPricePerMTokensInUSD);
        return DisplayCost(inputCost, outputCost);
    }

    private static string DisplayCost(double inputCost, double outputCost)
    {
        double totalCost = inputCost + outputCost;
        return $"Total cost: {FormatCost(inputCost)} + {FormatCost(outputCost)} = {FormatCost(totalCost)}";
    }

    private static double CalculateCost(int tokenCount, double pricePerMillionTokensInUSD)
    {
        return tokenCount / 1_000_000.0 * pricePerMillionTokensInUSD;
    }

    private static double EstimateCost(string text, double pricePerMillionTokensInUSD)
    {
        int tokenCount = CalculateTokenCount(text);
        return CalculateCost(tokenCount, pricePerMillionTokensInUSD);
    }

    private static int CalculateTokenCount(string text)
    {
        string[] words = text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return (int)Math.Ceiling(words.Length / WordsPerToken);
    }

    private static string FormatCost(double cost)
    {
        if (DisplayCostInDKK)
        {
            double costInDKK = cost * USDToDKKExchangeRate;
            return $"{costInDKK:0.00} kr.";
        }
        return $"${cost:0.00}";
    }
}

public interface IAiModel
{
    string ModelName { get; }
    string DisplayName { get; }
    IApiAccess ApiAccess { get; }
    int ContextWindow { get; }
    int MaxOutputTokens { get; }
    double InputPricePerMTokensInUSD { get; }
    double OutputPricePerMTokensInUSD { get; }
    DateTime TrainingCutoff { get; }
}

public interface IApiAccess
{
    string Endpoint { get; }
    string ApiKey { get; }
    ChatRoles ChatRoles { get; }
    IAiResponse RequestChatCompletion(AiRequest request);
}

public class ChatRoles(string user, string assistant, string system)
{
    public string UserRole { get; } = user;
    public string AssistantRole { get; } = assistant;
    public string SystemRole { get; } = system;
}

public static class ApiAccessUtils
{
    public static IAiResponse CallAPI<T>(string endpoint, List<Tuple<string,string>> headerData, AiRequest req) where T : IAiResponse, new()
    {
        using HttpClient client = new();
        foreach (var headerPair in headerData)
        {
            client.DefaultRequestHeaders.Add(headerPair.Item1, headerPair.Item2);
        }
        string requestBody = FormatJsonRequest(req);
        StringContent content = new(requestBody, Encoding.UTF8, "application/json");
        Console.WriteLine(content.ReadAsStringAsync().Result);
        HttpResponseMessage response = client.PostAsync(endpoint, content).Result;
        response.EnsureSuccessStatusCode();
        string responseString = response.Content.ReadAsStringAsync().Result;
        IAiResponse aiResponse = JsonSerializer.Deserialize<T>(responseString) ?? new T();
        return aiResponse;
    }

    private static string FormatJsonRequest(AiRequest req)
    {
        var requestBody = new
        {
            model = req.Model.ModelName,
            messages = req.MessagesToJsonObject(),
            max_tokens = req.MaxOutputTokens,
            stream = req.Stream,
            temperature = req.Temperature
        };
        return JsonSerializer.Serialize(requestBody);
    }
}

public class AiRequest(IAiModel model)
{
    public IAiModel Model{ get; set; } = model;
    public List<ChatMessage> Messages { get; set; } = [];
    public bool Stream { get; set; } = false;
    public int MaxOutputTokens { get; set; } = model.MaxOutputTokens;
    public double Temperature { get; set; } = 0.8;

    public void LoadTwoWayConversation(List<string> messages)
    {
        for (int i = 0; i < messages.Count; i++)
        {
            if (i % 2 == 0)
            {
                AddUserMessage(messages[i]);
            }
            else
            {
                AddAssistantMessage(messages[i]);
            }
        }
    }

    public void AddUserMessage(string content)
    {
        string userRole = Model.ApiAccess.ChatRoles.UserRole;
        Messages.Add(new ChatMessage(userRole, content));
    }

    public void AddAssistantMessage(string content)
    {
        string assistantRole = Model.ApiAccess.ChatRoles.AssistantRole;
        Messages.Add(new ChatMessage(assistantRole, content));
    }

    public List<object> MessagesToJsonObject()
    {
        var jsonMessages = new List<object>();
        foreach (ChatMessage message in Messages)
        {
            jsonMessages.Add(message.ToJsonObject());
        }
        return jsonMessages;
    }
}

public interface IAiResponse
{
    ChatMessage GetMessage();
    string GetId();
    string GetModel();
    string GetRequestType();
    string GetStopReason();
    string GetTimeCreated();
    int GetInputTokens();
    int GetOutputTokens();
}

public class AiErrorResponse(string errorMsg) : IAiResponse
{
    public ChatMessage GetMessage() => new("Error", errorMsg);
    public string GetId() => string.Empty;
    public string GetModel() => string.Empty;
    public string GetRequestType() => string.Empty;
    public string GetStopReason() => string.Empty;
    public string GetTimeCreated() => string.Empty;
    public int GetInputTokens() => 0;
    public int GetOutputTokens() => 0;
}

public class ChatMessage(string role, string content)
{
    public string Role = role;
    public string Content = content;

    public object ToJsonObject()
    {
        return new
        {
            role = Role,
            content = Content
        };
    }
}
