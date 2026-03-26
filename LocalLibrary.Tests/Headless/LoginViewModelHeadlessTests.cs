using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using LocalLibrary.Models;
using LocalLibrary.Services;
using LocalLibrary.ViewModels;

namespace LocalLibrary.Tests.Headless;

public class LoginViewModelHeadlessTests
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

    [AvaloniaFact]
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

    [AvaloniaFact]
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
        Assert.Equal("Member", currentUserService.UserRole);
        Assert.Equal("Member", callbackRole);
        Assert.NotNull(callbackUser);
    }
}
