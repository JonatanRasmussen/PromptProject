using System.Text;
using System.Text.Json;
namespace GlobalNameSpace;

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

public class ApiAccessEmpty : IApiAccess
{
    public string Endpoint { get; } = string.Empty;
    public string ApiKey { get; } = string.Empty;
    public ChatRoles ChatRoles { get; } = new(string.Empty, string.Empty, string.Empty);
    public IAiResponse RequestChatCompletion(AiRequest request)
    {
        return new AiErrorResponse(string.Empty);
    }
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