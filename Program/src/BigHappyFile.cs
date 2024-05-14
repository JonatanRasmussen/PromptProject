// This following is the content of file: BigHappyFile.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace GlobalNameSpace;

public class PromptManager
{
    public static void ExecutePrompt(IAiModel aiModel)
    {
        string prompt = Utils.ReadFile(MyLocalConfigs.InputFullFileName);
        AiRequest chatCompletion = new(aiModel);
        chatCompletion.AddUserMessage(prompt);
        try
        {
            IAiResponse aiResponse = aiModel.ApiAccess.RequestChatCompletion(chatCompletion);
            Console.WriteLine(aiResponse.GetMessage().Content);
            Utils.WriteFile(MyLocalConfigs.OutputFullFileName, aiResponse.GetMessage().Content);
            string totalCost = PromptCostCalculator.GetCost(aiModel, aiResponse.GetInputTokens(), aiResponse.GetOutputTokens());
            Console.WriteLine(totalCost);
        }
        catch (Exception ex)
        {
            Console.WriteLine("BudoError: " + ex.Message);
        }
    }
}

public static class Utils
{
    public static string FilePath(string fileNameAndExtension)
    {
        string currentDirectory = MyLocalConfigs.DirectoryFullPath;
        return Path.Combine(currentDirectory, fileNameAndExtension);
    }

    public static string ReadFile(string fileNameAndExtension)
    {
        string filePath = FilePath(fileNameAndExtension);
        using StreamReader reader = new(filePath);
        return reader.ReadToEnd();
    }

    public static void WriteFile(string fileNameAndExtension, string content)
    {
        string filePath = FilePath(fileNameAndExtension);
        using StreamWriter writer = new(filePath);
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
    public int MaxOutputTokens { get; set; } = 1024;
    public double Temperature { get; set; } = 1.0;

    public void AddUserMessage(string content)
    {
        string user = Model.ApiAccess.ChatRoles.UserRole;
        Messages.Add(new ChatMessage(user, content));
    }

    public List<object> MessagesToJsonObject()
    {
        var jsonMessages = new List<object>();
        foreach (var message in Messages)
        {
            jsonMessages.Add(message.ToJsonObject());
        }
        return jsonMessages;
    }
}

public interface IAiRequest
{
    string ModelName { get; set; }
    public List<ChatMessage> Messages { get; set; }
    public bool Stream { get; set; }
    public string[]? StopSequence { get; set; }
    public int MaxOutputTokens { get; set; }
    public double Temperature { get; set; }
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


