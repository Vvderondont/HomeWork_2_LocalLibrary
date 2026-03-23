using LocalLibrary.Models;
using LocalLibrary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace LocalLibrary.ViewModels;

public class LibrarianViewModel : ViewModelBase, ILogoutHandler
{
    private readonly LibraryService libraryService;
    private readonly Action saveData;
    private readonly Action logout;

    public ObservableCollection<Book> Books { get; set; }
    public ObservableCollection<Loan> Loans { get; set; }

    public LibrarianViewModel(LibraryData data, Action saveDataCallback, Action logoutCallback)
    {
        saveData = saveDataCallback;

        // services
        libraryService = new LibraryService(data);
        logout = logoutCallback;

        // expose to UI
        Books = new ObservableCollection<Book>(libraryService.Books);
        Loans = new ObservableCollection<Loan>(libraryService.Loans);
        EnsureLoanTitles();
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
        SaveData();
    }

    public void DeleteBook(Book book)
    {
        libraryService.Books.Remove(book);
        Books.Remove(book);
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
}


    

