using System.Text;
using System.Text.Json;
namespace GlobalNameSpace;

public static class UtilsForIO
{
    public static string ReadFile(string filePath, string fileNameAndExt)
    {
        string file = Path.Combine(filePath, fileNameAndExt);
        using StreamReader reader = new(file);
        return reader.ReadToEnd();
    }

    public static void WriteFile(string filePath, string fileNameAndExt, string content)
    {
        string file = Path.Combine(filePath, fileNameAndExt);
        using StreamWriter writer = new(file);
        writer.Write(content);
    }
}