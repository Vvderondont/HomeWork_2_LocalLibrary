using System.Collections.Generic;
using System.Linq;
using LocalLibrary.Models;

namespace LocalLibrary.Services;

public class AuthService
{
    private readonly LibraryData _libraryData;

    public AuthService(LibraryData libraryData)
    {
        _libraryData = libraryData;
    }

    public (bool success, string role, object? user) ValidateLogin(string username, string password)
    {
        // Check if credentials match librarian (top-level user in JSON)
        if (username == _libraryData.Username && password == _libraryData.Password)
        {
            return (true, "Librarian", null); // Return null or create Librarian object if needed
        }

        // Check if credentials match any member
        var member = _libraryData.Members?.FirstOrDefault(m => 
            m.Username == username && m.Password == password);

        if (member != null)
        {
            return (true, "Member", member);
        }

        return (false, "", null);
    }

    public (bool success, string message) RegisterMember(string username, string password)
    {
        // Check if username already exists (member)
        var existingMember = _libraryData.Members?.FirstOrDefault(m => m.Username == username);
        if (existingMember != null)
        {
            return (false, "Username already exists. Please choose another.");
        }

        // Check if username matches librarian
        if (username == _libraryData.Username)
        {
            return (false, "Username already exists. Please choose another.");
        }

        // Validate password length
        if (string.IsNullOrWhiteSpace(password) || password.Length < 4)
        {
            return (false, "Password must be at least 4 characters long.");
        }

        // Create new member
        var newMember = new Member
        {
            Username = username,
            Password = password,
            BorrowedBooks = new List<Book>()
        };

        _libraryData.Members?.Add(newMember);
        return (true, "Registration successful! You can now login.");
    }
}