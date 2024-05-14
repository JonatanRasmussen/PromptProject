// This following is the content of file: Program.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        PromptManager.ExecutePrompt(new OpenAI.Gpt4o());
/*         PromptManager.ExecutePrompt(new OpenAI.Gpt4Turbo());
        PromptManager.ExecutePrompt(new OpenAI.Gpt35Turbo());
        PromptManager.ExecutePrompt(new Anthropic.Claude3Haiku());
        PromptManager.ExecutePrompt(new Anthropic.Claude3Sonnet());
        PromptManager.ExecutePrompt(new Anthropic.Claude3Opus()); */
    }
}

public class MyLocalConfigs
{
    public static readonly string DirectoryFullPath = @"C:\Users\BudoB\OneDrive\Dokumenter Tekst\Programmering\PromptProject\Program\";
    public static readonly string ApiKeyAnthropic = Utils.ReadFile(Path.Combine("xkeys", "apikey_anthropic.txt"));
    public static readonly string ApiKeyOpenAI = Utils.ReadFile(Path.Combine("xkeys", "apikey_openai.txt"));
    public static readonly string InputFullFileName = "prompt.txt";
    public static readonly string OutputFullFileName = "response.txt";
}