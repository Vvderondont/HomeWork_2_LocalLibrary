using System.Text.Json;
using LocalLibrary.Models;
using System.IO;
using System;

namespace LocalLibrary.Services;

public class JsonDataService
{
    private const string LibrarianUsername = "admin";
    private const string DefaultLibrarianPassword = "admin123";
    private readonly string filePath;

    public JsonDataService()
    {
        filePath = ResolveDataFilePath();
    }

    public void SaveData(LibraryData data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, json);
    }

    public LibraryData LoadData()
    {
        if (!File.Exists(filePath))
        {
            return new LibraryData
            {
                Username = LibrarianUsername,
                Password = DefaultLibrarianPassword
            };
        }

        try
        {
            var json = File.ReadAllText(filePath);

            var data = JsonSerializer.Deserialize<LibraryData>(json) ?? new LibraryData();

            data.Username = LibrarianUsername;

            if (string.IsNullOrWhiteSpace(data.Password))
            {
                data.Password = DefaultLibrarianPassword;
            }

            data.Books ??= new();
            data.Members ??= new();
            data.Loans ??= new();

            return data;
        }
        catch
        {
            return new LibraryData
            {
                Username = LibrarianUsername,
                Password = DefaultLibrarianPassword
            };
        }
    }

    private static string ResolveDataFilePath()
    {
        const string relativeDataFilePath = "Data/library.json";

        var projectDirectory = FindProjectDirectory(AppContext.BaseDirectory);
        if (projectDirectory is not null)
        {
            return Path.Combine(projectDirectory.FullName, relativeDataFilePath);
        }

        return Path.Combine(AppContext.BaseDirectory, relativeDataFilePath);
    }

    private static DirectoryInfo? FindProjectDirectory(string startDirectory)
    {
        var current = new DirectoryInfo(startDirectory);

        while (current is not null)
        {
            if (current.GetFiles("*.csproj").Length > 0)
            {
                return current;
            }

            current = current.Parent;
        }

        return null;
    }
}