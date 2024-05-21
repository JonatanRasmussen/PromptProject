namespace GlobalNameSpace;

public class Program
{
    public static int AddNumbers(int a, int b)
    {
        return a + b;
    }

    public static void Main(string[] args)
    {
        LocalDirectory root = LocalDirectory.CreateWithSrcFolderAsRoot();
        root.PrintContent();
        string dirContent = root.DirContentToString();
        Console.WriteLine(dirContent);
        Console.WriteLine(dirContent.Length);
        //PromptManager.ExecutePrompt(new OpenAI.Gpt4o());
/*         PromptManager.ExecutePrompt(new OpenAI.Gpt4Turbo());
        PromptManager.ExecutePrompt(new OpenAI.Gpt35Turbo());
        PromptManager.ExecutePrompt(new Anthropic.Claude3Haiku());
        PromptManager.ExecutePrompt(new Anthropic.Claude3Sonnet());
        PromptManager.ExecutePrompt(new Anthropic.Claude3Opus()); */
    }
}

public class MyLocalConfigs
{
    public static readonly string AbsolutePath = @"C:\Users\BudoB\OneDrive\Dokumenter Tekst\Programmering\PromptProject\Program\";
    public static readonly string SrcFolderName = "src";
    public static readonly string SrcFolderPath = Path.Combine(AbsolutePath, SrcFolderName);
    public static readonly string ApiKeyAnthropic = Utils.ReadFile(Path.Combine("xkeys", "apikey_anthropic.txt"));
    public static readonly string ApiKeyOpenAI = Utils.ReadFile(Path.Combine("xkeys", "apikey_openai.txt"));
    public static readonly string InputFullFileName = "prompt.txt";
    public static readonly string OutputFullFileName = "response.md";
}