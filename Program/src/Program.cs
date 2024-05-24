namespace GlobalNameSpace;

public class Program
{
    public static int AddNumbers(int a, int b)
    {
        return a + b;
    }

    public static void Main(string[] args)
    {
        bool includeSrcCode = true;
        PromptManager.ExecutePromptPipeline(new OpenAI.Gpt4o(), includeSrcCode);
/*         PromptManager.ExecutePromptPipeline(new OpenAI.Gpt4Turbo(), includeSrcCode);
        PromptManager.ExecutePromptPipeline(new OpenAI.Gpt35Turbo(), includeSrcCode);
        PromptManager.ExecutePromptPipeline(new Anthropic.Claude3Haiku(), includeSrcCode);
        PromptManager.ExecutePromptPipeline(new Anthropic.Claude3Sonnet(), includeSrcCode);
        PromptManager.ExecutePromptPipeline(new Anthropic.Claude3Opus(), includeSrcCode); */
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
    public static readonly string ResponseName = "response.md";

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
    public static readonly string ApiKeyAnthropic = UtilsForIO.ReadFile(ProgramPaths.Xkeys, "apikey_anthropic.txt");
    public static readonly string ApiKeyOpenAI = UtilsForIO.ReadFile(ProgramPaths.Xkeys, "apikey_openai.txt");
    public static readonly string InputFullFileName = "prompt.txt";
    public static readonly string OutputFullFileName = "response.md";
}