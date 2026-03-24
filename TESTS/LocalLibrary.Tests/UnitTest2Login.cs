using LocalLibrary.Models;
using LocalLibrary.Services;
using LocalLibrary.ViewModels;

namespace LocalLibrary.Tests;

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

    [Fact]
    public async Task LoginCommand_EmptyCredentials_ShowsValidationMessage()
    {
        var authService = new AuthService(BuildLibraryData());
        var currentUserService = new CurrentUserService();
        var vm = new LoginViewModel(authService, currentUserService);

        vm.Username = string.Empty;
        vm.Password = string.Empty;

        await vm.LoginCommand.ExecuteAsync(null);

        Assert.Equal("Please enter username and password", vm.ErrorMessage);
        Assert.False(vm.IsSuccessMessage);
    }

    [Fact]
    public async Task LoginCommand_ValidMember_SetsCurrentUserAndInvokesCallback()
    {
        var authService = new AuthService(BuildLibraryData());
        var currentUserService = new CurrentUserService();

        string? callbackRole = null;
        object? callbackUser = null;

        var vm = new LoginViewModel(
            authService,
            currentUserService,
            onLoginSuccessCallback: (role, user) =>
            {
                callbackRole = role;
                callbackUser = user;
            });

        vm.Username = "member";
        vm.Password = "member";

        await vm.LoginCommand.ExecuteAsync(null);

        Assert.Equal("Login successful", vm.ErrorMessage);
        Assert.True(vm.IsSuccessMessage);
        Assert.Equal("member", currentUserService.LoggedInUsername);
        Assert.Equal("Member", currentUserService.UserRole);
        Assert.IsType<Member>(currentUserService.UserDetails);
        Assert.Equal("Member", callbackRole);
        Assert.IsType<Member>(callbackUser);
    }
}
