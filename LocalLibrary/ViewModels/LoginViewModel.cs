using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLibrary.Services;

namespace LocalLibrary.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly CurrentUserService _currentUserService;
        private readonly Action? _saveDataCallback;

        [ObservableProperty]
        private string _username = "";

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private string _errorMessage = "";

        [ObservableProperty]
        private bool _isRegisterMode = false;

        [ObservableProperty]
        private string _confirmPassword = "";

        public string LoginTitle => IsRegisterMode ? "Register" : "Login";
        public string ToggleButtonText => IsRegisterMode ? "Back to Login" : "Create Account";

        public LoginViewModel(AuthService authService, CurrentUserService currentUserService, Action? saveDataCallback = null)
        {
            _authService = authService;
            _currentUserService = currentUserService;
            _saveDataCallback = saveDataCallback;
        }

        [RelayCommand]
        private void ToggleRegisterMode()
        {
            IsRegisterMode = !IsRegisterMode;
            Username = "";
            Password = "";
            ConfirmPassword = "";
            ErrorMessage = "";
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter username and password";
                return;
            }

            var (success, role, user) = _authService.ValidateLogin(Username, Password);

            if (success)
            {
                _currentUserService.LoggedInUsername = Username;
                _currentUserService.UserRole = role;
                _currentUserService.UserDetails = user;

                ErrorMessage = "Login successful! Navigating...";
                await Task.Delay(1000);
                
                // TODO: Navigate based on role
                // if (role == "Member") -> navigate to CatalogView
                // if (role == "Librarian") -> navigate to LibrarianView
            }
            else
            {
                ErrorMessage = "Invalid username or password";
            }
        }

        [RelayCommand]
        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter username and password";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                return;
            }

            var (success, message) = _authService.RegisterMember(Username, Password);

            if (success)
            {
                ErrorMessage = message;
                // Save the new member to JSON
                _saveDataCallback?.Invoke();
                // Clear fields and switch back to login
                await Task.Delay(1500);
                IsRegisterMode = false;
                Username = "";
                Password = "";
                ConfirmPassword = "";
            }
            else
            {
                ErrorMessage = message;
            }
        }
    }
}
