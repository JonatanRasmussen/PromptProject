using System.Text;
using System.Text.Json;
namespace GlobalNameSpace;

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
