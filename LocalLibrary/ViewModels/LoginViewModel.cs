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
        private readonly Action<string, object?>? _onLoginSuccessCallback;

        [ObservableProperty]
        private string _username = "";

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private string _errorMessage = "";

        [ObservableProperty]
        private bool _isSuccessMessage;

        [ObservableProperty]
        private bool _isRegisterMode = false;

        [ObservableProperty]
        private string _confirmPassword = "";

        public string LoginTitle => IsRegisterMode ? "Register" : "Sign In";
        public string ToggleButtonText => IsRegisterMode ? "Login" : "Register";
        public string MessageColor => IsSuccessMessage ? "Green" : "Red";

        public LoginViewModel(
            AuthService authService,
            CurrentUserService currentUserService,
            Action? saveDataCallback = null,
            Action<string, object?>? onLoginSuccessCallback = null)
        {
            _authService = authService;
            _currentUserService = currentUserService;
            _saveDataCallback = saveDataCallback;
            _onLoginSuccessCallback = onLoginSuccessCallback;
        }

        partial void OnIsRegisterModeChanged(bool value)
        {
            OnPropertyChanged(nameof(LoginTitle));
            OnPropertyChanged(nameof(ToggleButtonText));
        }

        partial void OnIsSuccessMessageChanged(bool value)
        {
            OnPropertyChanged(nameof(MessageColor));
        }

        [RelayCommand]
        private void ToggleRegisterMode()
        {
            IsRegisterMode = !IsRegisterMode;
            Username = "";
            Password = "";
            ConfirmPassword = "";
            ErrorMessage = "";
            IsSuccessMessage = false;
        }

        public void PrepareForNewLogin()
        {
            IsRegisterMode = false;
            Username = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            ErrorMessage = string.Empty;
            IsSuccessMessage = false;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter username and password";
                IsSuccessMessage = false;
                return;
            }

            var (success, role, user) = _authService.ValidateLogin(Username, Password);

            if (success)
            {
                _currentUserService.LoggedInUsername = Username;
                _currentUserService.UserRole = role;
                _currentUserService.UserDetails = user;

                ErrorMessage = "Login successful";
                IsSuccessMessage = true;
                await Task.Delay(1000);
                _onLoginSuccessCallback?.Invoke(role, user);
            }
            else
            {
                ErrorMessage = "Invalid username or password";
                IsSuccessMessage = false;
            }
        }

        [RelayCommand]
        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter username and password";
                IsSuccessMessage = false;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                IsSuccessMessage = false;
                return;
            }

            var (success, message) = _authService.RegisterMember(Username, Password);

            if (success)
            {
                ErrorMessage = message;
                IsSuccessMessage = true;
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
                IsSuccessMessage = false;
            }
        }
    }
}
