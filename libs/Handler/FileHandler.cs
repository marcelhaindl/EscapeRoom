using System.Reflection.Metadata.Ecma335;

namespace libs;

using Newtonsoft.Json;

public static class FileHandler
{
    // File path for the JSON file
    private static string filePath;
    // Environment variable name for the file path
    private readonly static string envVar = "ESCAPE_ROOM_PATH";

    // Static constructor to initialize the file path
    static FileHandler()
    {
        Initialize();
    }

    // Initialize the file path from the environment variable
    private static void Initialize()
    {
        if (Environment.GetEnvironmentVariable(envVar) != null)
        {
            filePath = Environment.GetEnvironmentVariable(envVar);
        }
    }

    // Method to read and deserialize JSON data from the file
    public static dynamic ReadJson()
    {
        // Get the file path from the environment variable
        string filePath = Environment.GetEnvironmentVariable("ESCAPE_ROOM_PATH");

        // Check if the file path is provided
        if (string.IsNullOrEmpty(filePath))
        {
            throw new InvalidOperationException("JSON file path not provided in environment variable");
        }

        try
        {
            // Read the JSON content from the file
            string jsonContent = File.ReadAllText(filePath);
            // Deserialize the JSON content into a dynamic object
            dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);
            return jsonData;
        }
        catch (FileNotFoundException)
        {
            // Throw an exception if the file is not found
            throw new FileNotFoundException($"JSON file not found at path: {filePath}");
        }
        catch (Exception ex)
        {
            // Throw an exception if there is an error reading the file
            throw new Exception($"Error reading JSON file: {ex.Message}");
        }
    }
}
