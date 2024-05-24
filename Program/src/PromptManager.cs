using System.Text;
using System.Text.Json;
namespace GlobalNameSpace;

public static class PromptManager
{
    public static void ExecutePromptPipeline(IAiModel aiModel, bool includeSrcCode)
    {
        long promptNumber = GeneratePromptNumber();
        string prompt = LoadPrompt();
        if (includeSrcCode)
        {
            prompt = AppendSrcCodeToPrompt(prompt);
        }
        ArchivePrompt(promptNumber, prompt);
        AiRequest request = PreparePrompt(aiModel, prompt);
        IAiResponse response = SubmitPrompt(aiModel, request);
        SaveAsResponseMd(response);
        ArchiveResponse(promptNumber, response);
        ArchiveMetadata(promptNumber, request, response);
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
        return UtilsForIO.ReadFile(ProgramPaths.Prompt, ProgramFiles.Prompt);
    }

    private static string AppendSrcCodeToPrompt(string prompt)
    {
        string fullSrcCode = LocalDirectory.SrcCodeToString();
        return $"{prompt}\n\n{fullSrcCode}";
    }

    private static void ArchivePrompt(long promptNumber, string prompt)
    {
        string nameAndExt = ProgramFiles.FormatInputName(promptNumber);
        UtilsForIO.WriteFile(ProgramPaths.Archive, nameAndExt, prompt);
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

    private static void ArchiveResponse(long promptNumber, IAiResponse response)
    {
        string completion = response.GetMessage().Content;
        string nameAndExt = ProgramFiles.FormatOutputName(promptNumber);
        UtilsForIO.WriteFile(ProgramPaths.Archive, nameAndExt, completion);
    }

    private static void SaveAsResponseMd(IAiResponse response)
    {
        string completion = response.GetMessage().Content;
        string nameAndExt = ProgramFiles.ResponseName;
        UtilsForIO.WriteFile(ProgramPaths.Response, nameAndExt, completion);
    }

    private static void ArchiveMetadata(long promptNumber, AiRequest request, IAiResponse response)
    {
        AiMetadata summary = new(request, response);
        string summaryJson = summary.ToJson();
        string nameAndExt = ProgramFiles.FormatMetadataName(promptNumber);
        UtilsForIO.WriteFile(ProgramPaths.Archive, nameAndExt, summaryJson);
    }

    private static void PrintCost(IAiModel aiModel, IAiResponse response)
    {
        int inputTokens = response.GetInputTokens();
        int outputTokens = response.GetOutputTokens();
        string totalCost = PromptCostCalculator.GetCost(aiModel, inputTokens, outputTokens);
        Console.WriteLine(totalCost);
    }
}
