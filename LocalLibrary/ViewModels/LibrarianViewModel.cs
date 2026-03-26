using LocalLibrary.Models;
using LocalLibrary.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
namespace LocalLibrary.ViewModels;

public class LibrarianViewModel : ViewModelBase, ILogoutHandler
{
    private readonly LibraryService libraryService;
    private readonly Action saveData;
    private readonly Action logout;

    public ObservableCollection<Book> Books { get; set; }
    public ObservableCollection<Book> FilteredBooks { get; } = new();
    public ObservableCollection<Loan> Loans { get; set; }
    public bool ShowEmptyState { get; private set; }
    public string EmptyStateMessage { get; private set; } = "No books in the library yet.";
    public string StatusMessage { get; private set; } = string.Empty;
    public string StatusColor { get; private set; } = "ForestGreen";

    public string NewBookTitle { get; set; } = string.Empty;
    public string NewBookAuthor { get; set; } = string.Empty;
    public string NewBookIsbn { get; set; } = string.Empty;
    public string NewBookDescription { get; set; } = string.Empty;

    public IRelayCommand AddBookCommand { get; }
    public IRelayCommand DeleteSelectedBookCommand { get; }
    public IRelayCommand EditSelectedBookCommand { get; }
    public IRelayCommand ClearSearchCommand { get; }

    private string searchText = string.Empty;
    public string SearchText
    {
        get => searchText;
        set
        {
            if (searchText == value)
            {
                return;
            }

            searchText = value;
            OnPropertyChanged(nameof(SearchText));
            RefreshFilteredBooks();
        }
    }

    public LibrarianViewModel(LibraryData data, Action saveDataCallback, Action logoutCallback)
    {
        saveData = saveDataCallback;

        // services
        libraryService = new LibraryService(data);
        logout = logoutCallback;

        // expose to UI
        Books = new ObservableCollection<Book>(libraryService.Books);
        Loans = new ObservableCollection<Loan>(libraryService.Loans);
        RefreshFilteredBooks();
        EnsureLoanTitles();

        AddBookCommand = new RelayCommand(AddBookFromForm);
        DeleteSelectedBookCommand = new RelayCommand(DeleteSelectedBook);
        EditSelectedBookCommand = new RelayCommand(EditSelectedBook);
        ClearSearchCommand = new RelayCommand(ClearSearch);
    }

    public void SaveData()
    {
        // Save the full shared model so members/admin password are preserved.
        saveData();
    }

    public void RefreshLoans()
    {
        EnsureLoanTitles();
        Loans.Clear();
        foreach (var loan in libraryService.Loans)
        {
            Loans.Add(loan);
        }
    }

    public void AddBook(Book book)
    {
        libraryService.Books.Add(book);
        Books.Add(book);
        RefreshFilteredBooks();
        SaveData();
    }

    public void DeleteBook(Book book)
    {
        libraryService.Books.Remove(book);
        Books.Remove(book);
        RefreshFilteredBooks();
        SaveData();
    }

    private Book? selectedBook;
    public Book? SelectedBook
    {
        get => selectedBook;
        set
        {
            if (selectedBook != value)
            {
                selectedBook = value;
                OnPropertyChanged(nameof(SelectedBook));
            }
        }
    }

    public void DeleteBook()
    {
        if (SelectedBook != null)
        {
            libraryService.Books.Remove(SelectedBook);
            Books.Remove(SelectedBook);
            RefreshFilteredBooks();
            SaveData();
        }
    }

    public void EditBook()
    {
        if (SelectedBook != null)
        {
            // Find the book in libraryService.Books and ensure it's updated
            var bookToUpdate = libraryService.Books.FirstOrDefault(b => b.ISBN == SelectedBook.ISBN);
            if (bookToUpdate != null)
            {
                bookToUpdate.Title = SelectedBook.Title;
                bookToUpdate.Author = SelectedBook.Author;
                bookToUpdate.Description = SelectedBook.Description;
                bookToUpdate.IsBorrowed = SelectedBook.IsBorrowed;
            }

            RefreshFilteredBooks();
            SaveData();
        }
    }

    public void EditBookDescription(Book book, string newDescription)
    {
        if (book != null)
        {
            var bookToUpdate = libraryService.Books.FirstOrDefault(b => b.ISBN == book.ISBN);
            if (bookToUpdate != null)
            {
                bookToUpdate.Description = newDescription;
                SaveData();
            }
        }
    }

    public void BorrowBook(Member member, Book book)
    {
        libraryService.BorrowBook(member,book);
        SaveData();
        RefreshLoans();
    }

    public void ReturnBook(Member member, Book book)
    {
        libraryService.ReturnBook(member, book);
        SaveData();

          RefreshLoans();
    }
    

    public void Logout()
    {
        logout();
    }

    private void EnsureLoanTitles()
    {
        foreach (var loan in libraryService.Loans)
        {
            if (!string.IsNullOrWhiteSpace(loan.BookTitle))
            {
                continue;
            }

            var book = libraryService.Books.FirstOrDefault(b =>
                string.Equals(b.ISBN, loan.BookISBN, StringComparison.OrdinalIgnoreCase));

            loan.BookTitle = book?.Title ?? "Unknown title";
        }
    }

    public void ClearSearch()
    {
        SearchText = string.Empty;
        RefreshFilteredBooks();
    }

    private void AddBookFromForm()
    {
        var title = NewBookTitle?.Trim();
        var author = NewBookAuthor?.Trim();
        var isbn = NewBookIsbn?.Trim();
        var description = NewBookDescription?.Trim();

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(isbn))
        {
            SetStatus("Please complete Title, Author, and ISBN to add a book.", true);
            return;
        }

        if (!IsValidIsbn13(isbn))
        {
            SetStatus("ISBN must contain exactly 13 digits.", true);
            return;
        }

        var exists = Books.Any(b => string.Equals(b.ISBN, isbn, StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            SetStatus("A book with that ISBN already exists.", true);
            return;
        }

        AddBook(new Book
        {
            Title = title,
            Author = author,
            ISBN = isbn,
            Description = description,
            IsBorrowed = false
        });

        NewBookTitle = string.Empty;
        NewBookAuthor = string.Empty;
        NewBookIsbn = string.Empty;
        NewBookDescription = string.Empty;
        OnPropertyChanged(nameof(NewBookTitle));
        OnPropertyChanged(nameof(NewBookAuthor));
        OnPropertyChanged(nameof(NewBookIsbn));
        OnPropertyChanged(nameof(NewBookDescription));

        SetStatus("Book added successfully.", false);
    }

    private void DeleteSelectedBook()
    {
        if (SelectedBook is null)
        {
            SetStatus("Select a book to delete.", true);
            return;
        }

        DeleteBook(SelectedBook);
        SetStatus("Book deleted successfully.", false);
    }

    private void EditSelectedBook()
    {
        if (SelectedBook is null)
        {
            SetStatus("Please select a book to edit.", true);
            return;
        }

        var isbn = SelectedBook.ISBN?.Trim();
        if (!IsValidIsbn13(isbn))
        {
            SetStatus("ISBN must contain exactly 13 digits.", true);
            return;
        }

        var duplicateCount = Books.Count(b => string.Equals(b.ISBN, isbn, StringComparison.OrdinalIgnoreCase));
        if (duplicateCount > 1)
        {
            SetStatus("A book with that ISBN already exists.", true);
            return;
        }

        EditBook();
        SetStatus("Book changes saved successfully.", false);
    }

    private static bool IsValidIsbn13(string? isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn) || isbn.Length != 13)
        {
            return false;
        }

        return isbn.All(char.IsDigit);
    }

    private void SetStatus(string message, bool isError)
    {
        StatusMessage = message;
        StatusColor = isError ? "IndianRed" : "ForestGreen";
        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(StatusColor));
    }

    private void RefreshFilteredBooks()
    {
        var term = SearchText?.Trim();

        var filtered = string.IsNullOrWhiteSpace(term)
            ? libraryService.Books
            : libraryService.Books.Where(b =>
                (b.Title?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (b.Author?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (b.ISBN?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));

        FilteredBooks.Clear();
        foreach (var book in filtered)
        {
            FilteredBooks.Add(book);
        }

        if (SelectedBook is not null && !FilteredBooks.Contains(SelectedBook))
        {
            SelectedBook = null;
        }

        ShowEmptyState = FilteredBooks.Count == 0;
        EmptyStateMessage = string.IsNullOrWhiteSpace(term)
            ? "No books in the library yet."
            : "No books match your search.";
        OnPropertyChanged(nameof(ShowEmptyState));
        OnPropertyChanged(nameof(EmptyStateMessage));
    }
}


    

