using System.Collections.Generic;
using System.Linq;
using LocalLibrary.Models;

namespace LocalLibrary.Services;

public class AuthService
{
    private const string LibrarianUsername = "admin";
    private readonly LibraryData _libraryData;

    public AuthService(LibraryData libraryData)
    {
        _libraryData = libraryData;
    }

    public (bool success, string role, object? user) ValidateLogin(string username, string password)
    {
        // Only admin is allowed as librarian account.
        if (string.Equals(username, LibrarianUsername, System.StringComparison.OrdinalIgnoreCase) &&
            string.Equals(password, _libraryData.Password, System.StringComparison.Ordinal))
        {
            return (true, "Librarian", null);
        }

        var member = _libraryData.Members?.FirstOrDefault(m =>
            string.Equals(m.Username, username, System.StringComparison.OrdinalIgnoreCase) &&
            string.Equals(m.Password, password, System.StringComparison.Ordinal));

        if (member != null)
        {
            return (true, "Member", member);
        }

        return (false, "", null);
    }

    public (bool success, string message) RegisterMember(string username, string password)
    {
        var existingMember = _libraryData.Members?.FirstOrDefault(m =>
            string.Equals(m.Username, username, System.StringComparison.OrdinalIgnoreCase));
        if (existingMember != null)
        {
            return (false, "Username already exists. Please choose another.");
        }

        if (string.Equals(username, LibrarianUsername, System.StringComparison.OrdinalIgnoreCase))
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