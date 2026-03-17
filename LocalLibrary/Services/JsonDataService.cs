using System.Text.Json;
using LocalLibrary.Models;
using System.IO;

namespace LocalLibrary.Services;

public class JsonDataService
{
    private readonly string filePath = "Data/library.json";

    public void SaveData(LibraryData data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Directory.CreateDirectory("Data");
        File.WriteAllText(filePath, json);
    }

    public LibraryData LoadData()
    {
        if (!File.Exists(filePath))
        {
            return new LibraryData();
        }

        try
        {
            var json = File.ReadAllText(filePath);

            var data = JsonSerializer.Deserialize<LibraryData>(json);

            return data ?? new LibraryData();
        }
        catch
        {
            return new LibraryData();
        }
    }
}