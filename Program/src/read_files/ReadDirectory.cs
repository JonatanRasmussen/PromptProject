namespace GlobalNameSpace;

public class TraverseDirectory
{
    public static Dictionary<string, string> FetchFileContent(string path)
    {
        Dictionary<string, string> contents = [];
        string fullPath = Path.Combine(MyLocalConfigs.AbsolutePath, path);
        string[] files = Directory.GetFiles(fullPath, "*.cs");
        foreach (string file in files)
        {
            try
            {
                string fileName = Path.GetFileName(file);
                string content = File.ReadAllText(file);
                contents[$"{path}\\{fileName}"] = content;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"BudoError: Access denied to {file}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BudoError: Error reading file {file}: {ex.Message}");
            }
        }
        string[] folders = Directory.GetDirectories(fullPath);
        foreach (string folder in folders)
        {
            Console.WriteLine(folder);
            try
            {
                string folderName = Path.GetFileName(folder);
                string newPath = Path.Combine(path, folderName);
                var newContents = FetchFileContent(newPath);
                foreach (var kvp in newContents)
                {
                    contents[kvp.Key] = kvp.Value;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"BudoError: Access denied to {folder}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BudoError: Error reading folder {folder}: {ex.Message}");
            }
        }
        return contents;
    }

    public static string CreateString(Dictionary<string, string> fileContents)
    {
        string concattedString = string.Empty;
        foreach (var entry in fileContents)
        {
            string fileHeader = $"\n\n\n// The following is the content of file: {entry.Key}\n";
            concattedString += fileHeader + entry.Value;
        }
        return concattedString;
    }

    public static void PrintFileContents(Dictionary<string, string> fileContents)
    {
        foreach (var entry in fileContents)
        {
            Console.WriteLine($"File: {entry.Key}");
            Console.WriteLine("Content:");
            Console.WriteLine(entry.Value[0..60]);
            Console.WriteLine("--------");
        }
    }
}
