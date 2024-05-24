using System.Text;
using System.Text.Json;
namespace GlobalNameSpace;

public static class PromptCostCalculator
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