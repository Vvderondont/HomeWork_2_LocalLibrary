using System.Collections.Generic;
using LocalLibrary.Models;
using LocalLibrary.Services;

namespace LocalLibrary.Tests.Unit;

public class LoginTests
{
    private static LibraryData BuildLibraryData() => new()
    {
        Username = "admin",
        Password = "admin123",
        Members = new List<Member>
        {
            new() { Username = "member", Password = "member" },
            new() { Username = "gojo", Password = "gojo123" }
        }
    };

    [Fact]
    public void ValidateLogin_LibrarianCredentials_AreAccepted()
    {
        var authService = new AuthService(BuildLibraryData());

        var (success, role, user) = authService.ValidateLogin("AdMiN", "admin123");

        Assert.True(success);
        Assert.Equal("Librarian", role);
        Assert.Null(user);
    }

    [Fact]
    public void ValidateLogin_MemberCredentials_AreAccepted()
    {
        var authService = new AuthService(BuildLibraryData());

        var (success, role, user) = authService.ValidateLogin("gojo", "gojo123");

        Assert.True(success);
        Assert.Equal("Member", role);
        Assert.IsType<Member>(user);
        Assert.Equal("gojo", ((Member)user!).Username);
    }

    [Fact]
    public void ValidateLogin_InvalidPassword_IsRejected()
    {
        var authService = new AuthService(BuildLibraryData());

        var (success, role, user) = authService.ValidateLogin("member", "wrong-password");

        Assert.False(success);
        Assert.Equal(string.Empty, role);
        Assert.Null(user);
    }
}
