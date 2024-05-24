using System.Text;
using System.Text.Json;
namespace GlobalNameSpace;

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

public class AiEmptyModel : IAiModel
{
    public string ModelName { get; } = string.Empty;
    public string DisplayName { get; } = string.Empty;
    public IApiAccess ApiAccess { get; } = new ApiAccessEmpty();
    public int ContextWindow { get; }
    public int MaxOutputTokens { get; }
    public double InputPricePerMTokensInUSD { get; }
    public double OutputPricePerMTokensInUSD { get; }
    public DateTime TrainingCutoff { get; }
}