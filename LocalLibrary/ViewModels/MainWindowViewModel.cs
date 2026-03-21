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

    public MemberViewModel MemberViewModel { get; private set; }

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
        var bootstrapMember = EnsureMemberExists();
        LibraryService = new LibraryService(LibraryData);
        LoginViewModel = new LoginViewModel(authService, currentUserService, SaveData, OnLoginSuccess);
        MemberViewModel = new MemberViewModel(LibraryService, bootstrapMember, SaveData);
        LibrarianViewModel = new LibrarianViewModel(LibraryData);
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

            MemberViewModel = new MemberViewModel(LibraryService, member, SaveData);
            OnPropertyChanged(nameof(MemberViewModel));
            CurrentViewModel = MemberViewModel;
        }
    }

    private Member EnsureMemberExists()
    {
        if (LibraryData.Members.Count > 0)
        {
            return LibraryData.Members[0];
        }

        var defaultMember = new Member
        {
            Username = "member",
            Password = "member"
        };

        LibraryData.Members.Add(defaultMember);

        if (LibraryData.Books.Count == 0)
        {
            LibraryData.Books.AddRange(
            [
                new Book
                {
                    Title = "Clean Code",
                    Author = "Robert C. Martin",
                    ISBN = "9780132350884",
                    Description = "A handbook of agile software craftsmanship.",
                    IsBorrowed = false
                },
                new Book
                {
                    Title = "The Pragmatic Programmer",
                    Author = "Andrew Hunt, David Thomas",
                    ISBN = "9780135957059",
                    Description = "Classic guide to software engineering practices.",
                    IsBorrowed = false
                },
                new Book
                {
                    Title = "Refactoring",
                    Author = "Martin Fowler",
                    ISBN = "9780134757599",
                    Description = "Improving the design of existing code.",
                    IsBorrowed = false
                }
            ]);
        }

        return defaultMember;
    }
}