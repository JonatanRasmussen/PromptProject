using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace GlobalNameSpace;

public class Program
{
    public static int AddNumbers(int a, int b)
    {
        return a + b;
    }

    public static void Main(string[] args)
    {
        IAiModel model = new OpenAI.Gpt4o();
        bool includeSrcCode = false;
        PromptManager promptManager = new(model, includeSrcCode);
        //await promptManager.Stream();
        promptManager.ExecutePromptPipeline();
/*         PromptManager.ExecutePromptPipeline(new OpenAI.Gpt4Turbo(), includeSrcCode);
        PromptManager.ExecutePromptPipeline(new OpenAI.Gpt35Turbo(), includeSrcCode);
        PromptManager.ExecutePromptPipeline(new Anthropic.Claude3Haiku(), includeSrcCode);
        PromptManager.ExecutePromptPipeline(new Anthropic.Claude3Sonnet(), includeSrcCode);
        PromptManager.ExecutePromptPipeline(new Anthropic.Claude3Opus(), includeSrcCode); */
    }

    private static readonly HttpClient client = new HttpClient();

    static async Task Testyyy()
    {
        string apiKey = MyLocalConfigs.ApiKeyOpenAI;
        string apiUrl = "https://api.openai.com/v1/chat/completions";

        var requestBody = new
        {
            model = "gpt-4o",
            messages = new[]
            {
                new { role = "user", content = "Hey this is a test." }
            },
            stream = true
        };

        using HttpClient client = new();
        string requestJson = JsonSerializer.Serialize(requestBody);
        HttpRequestMessage requestMessage = new(HttpMethod.Post, apiUrl);
        requestMessage.Headers.Add("Authorization", $"Bearer {apiKey}");
        requestMessage.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        using Stream responseStream = await response.Content.ReadAsStreamAsync();
        using StreamReader streamReader = new(responseStream);

        while (!streamReader.EndOfStream)
        {
            var line = await streamReader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(line) && line != "data: [DONE]")
            {
                Console.WriteLine(line);
            }
        }
    }
}

public class ProgramPaths
{
    public static readonly string Root = MyLocalConfigs.AbsolutePath;
    public static readonly string Prompt = Root;
    public static readonly string Response = Root;
    public static readonly string Src = Path.Combine(Root, "src");
    public static readonly string Xkeys = Path.Combine(Root, "xkeys");
    public static readonly string Userdata = Path.Combine(Root, "userdata");
    public static readonly string Archive = Path.Combine(Userdata, "archive");
}

public class ProgramFiles
{
    public static readonly string Prompt = "prompt.txt";
    public static readonly string Response = "response.md";

    public static string FormatInputName(long promptNumber)
    {
        string extension = ".txt";
        return $"{promptNumber}_0{extension}";
    }

    public static string FormatOutputName(long promptNumber)
    {
        string extension = ".md";
        return $"{promptNumber}_1{extension}";
    }

    public static string FormatMetadataName(long promptNumber)
    {
        string extension = ".json";
        return $"{promptNumber}_0x{extension}";
    }
}

public class MyLocalConfigs
{
    public static readonly string AbsolutePath = @"C:\Users\BudoB\OneDrive\Dokumenter Tekst\Programmering\PromptProject\Program\";
    public static readonly string ApiKeyAnthropic = Utils.ReadFile(ProgramPaths.Xkeys, "apikey_anthropic.txt");
    public static readonly string ApiKeyOpenAI = Utils.ReadFile(ProgramPaths.Xkeys, "apikey_openai.txt");
    public static readonly string InputFullFileName = "prompt.txt";
    public static readonly string OutputFullFileName = "response.md";
}