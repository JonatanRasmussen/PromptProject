using System.Text;
using System.Text.Json;
namespace GlobalNameSpace;

public class AiMetadata(AiRequest req, IAiResponse response)
{
    // Properties initialized via constructor syntax
    public string Model { get; set; } = response.GetModel();
    public string Id { get; set; } = response.GetId();
    public string RequestType { get; set; } = response.GetRequestType();
    public string StopReason { get; set; } = response.GetStopReason();
    public string TimeCreated { get; set; } = response.GetTimeCreated();
    public int InputTokens { get; set; } = response.GetInputTokens();
    public int OutputTokens { get; set; } = response.GetOutputTokens();
    public int MaxTokens { get; set; } = req.MaxOutputTokens;
    public double Temperature { get; set; } = req.Temperature;
    public bool Stream { get; set; } = req.Stream;

    public static AiMetadata CreateEmpty()
    {
        IAiModel model = new AiEmptyModel();
        AiRequest req = new(model);
        IAiResponse response = new AiErrorResponse(string.Empty);
        return new(req, response);
    }

    // Method to convert the object to JSON string
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    // Method to create an object from JSON string
    public static AiMetadata FromJson(string json)
    {
        return JsonSerializer.Deserialize<AiMetadata>(json) ?? CreateEmpty();
    }
}

