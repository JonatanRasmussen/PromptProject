using System.Text;
using System.Text.Json;
namespace GlobalNameSpace;

public class PromptManager(IAiModel aiModel, bool includeSrcCode)
{
    public long PromptNumber { get; private set; }
    public string Prompt { get; private set; } = string.Empty;
    public AiRequest Request { get; private set; } = new(aiModel);
    public IAiResponse Response { get; private set; } = new AiErrorResponse(string.Empty);
    public IAiModel AiModel { get; } = aiModel;
    public bool IncludeSrcCode { get; } = includeSrcCode;

    public async Task Stream()
    {
        Prompt = LoadPrompt();
        Request = PreparePrompt();
        try
        {
            Request.Stream = true;
            await OpenAI.RequestChatCompletionStream(Request);
        }
        catch (Exception ex)
        {
            Console.WriteLine("BudoError: " + ex.Message);
        }
    }

    public void ExecutePromptPipeline()
    {
        PromptNumber = GeneratePromptNumber();
        Prompt = LoadPrompt();
        if (IncludeSrcCode)
        {
            AppendSrcCodeToPrompt();
        }
        ArchivePrompt();
        Request = PreparePrompt();
        Response = SubmitPrompt();
        ArchiveResponse();
        ArchiveMetadata();
        PrintCost();
    }

    private static long GeneratePromptNumber()
    {
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
        long epochTimeSeconds = currentTime.ToUnixTimeSeconds();
        long numberToCountDownFrom = 10_000_000_000;
        return numberToCountDownFrom - epochTimeSeconds;
    }

    private static string LoadPrompt()
    {
        return Utils.ReadFile(ProgramPaths.Prompt, ProgramFiles.Prompt);
    }

    private void AppendSrcCodeToPrompt()
    {
        string fullSrcCode = LocalDirectory.SrcCodeToString();
        Prompt = $"{Prompt}\n\n{fullSrcCode}";
    }

    private void ArchivePrompt()
    {
        string nameAndExt = ProgramFiles.FormatInputName(PromptNumber);
        Utils.WriteFile(ProgramPaths.Archive, nameAndExt, Prompt);
    }

    private AiRequest PreparePrompt()
    {
        AiRequest aiRequest = new(AiModel);
        aiRequest.AddUserMessage(Prompt);
        return aiRequest;
    }

    private IAiResponse SubmitPrompt()
    {
        try
        {
            IAiResponse response = AiModel.ApiAccess.RequestChatCompletion(Request);
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine("BudoError: " + ex.Message);
            return new AiErrorResponse(ex.Message);
        }
    }

    private void ArchiveResponse()
    {
        string completion = Response.GetMessage().Content;
        string nameAndExt = ProgramFiles.FormatOutputName(PromptNumber);
        Utils.WriteFile(ProgramPaths.Archive, nameAndExt, completion);
    }

    private void ArchiveMetadata()
    {
        AiMetadata summary = new(Request, Response);
        string summaryJson = summary.ToJson();
        string nameAndExt = ProgramFiles.FormatMetadataName(PromptNumber);
        Utils.WriteFile(ProgramPaths.Archive, nameAndExt, summaryJson);
    }

    private void PrintCost()
    {
        int inputTokens = Response.GetInputTokens();
        int outputTokens = Response.GetOutputTokens();
        string totalCost = PromptCostCalculator.GetCost(AiModel, inputTokens, outputTokens);
        Console.WriteLine(totalCost);
    }
}
