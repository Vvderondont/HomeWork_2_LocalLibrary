using LocalLibrary.Services;
using LocalLibrary.Models;
using System.Linq;
namespace LocalLibrary.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    private readonly JsonDataService dataService;
    private readonly AuthService authService;
    private readonly CurrentUserService currentUserService;

    private ViewModelBase _currentViewModel = null!;

    public LoginViewModel LoginViewModel { get; }

    public MemberViewModel? MemberViewModel { get; private set; }

    public LibrarianViewModel LibrarianViewModel { get; }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public LibraryService LibraryService { get; }

    public LibraryData LibraryData { get; set; }

    public MainWindowViewModel()
    {
        dataService = new JsonDataService();

        LibraryData = dataService.LoadData();

        currentUserService = new CurrentUserService();
        authService = new AuthService(LibraryData);
        LibraryService = new LibraryService(LibraryData);
        
        LoginViewModel = new LoginViewModel(authService, currentUserService, SaveData, OnLoginSuccess);
        LibrarianViewModel = new LibrarianViewModel(LibraryData, SaveData, OnLogout);
        CurrentViewModel = LoginViewModel;
    }

    public void SaveData()
    {
        dataService.SaveData(LibraryData);
    }

    private void OnLoginSuccess(string role, object? user)
    {
        if (role == "Librarian")
        {
            LibrarianViewModel.RefreshLoans();
            CurrentViewModel = LibrarianViewModel;
            return;
        }

        if (role == "Member")
        {
            var member = user as Member;

            if (member is null)
            {
                var username = currentUserService.LoggedInUsername ?? string.Empty;
                member = LibraryData.Members.FirstOrDefault(m =>
                    string.Equals(m.Username, username, System.StringComparison.OrdinalIgnoreCase));
            }

            if (member is null)
            {
                return;
            }

            MemberViewModel = new MemberViewModel(LibraryService, member, SaveData, OnLogout);
            OnPropertyChanged(nameof(MemberViewModel));
            CurrentViewModel = MemberViewModel;
        }
    }

    private void OnLogout()
    {
        currentUserService.LoggedInUsername = null;
        currentUserService.UserRole = null;
        currentUserService.UserDetails = null;
        LoginViewModel.PrepareForNewLogin();
        CurrentViewModel = LoginViewModel;
    }
}