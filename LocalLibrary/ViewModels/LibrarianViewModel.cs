using LocalLibrary.Models;
using LocalLibrary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace LocalLibrary.ViewModels;

public class LibrarianViewModel : ViewModelBase, ILogoutHandler
{
    private readonly LibraryService libraryService;
    private readonly JsonDataService dataService;
    private readonly Action logout;

    public ObservableCollection<Book> Books { get; set; }

    public LibrarianViewModel(LibraryData data, Action logoutCallback)
    {   
        //services
        libraryService = new LibraryService(data);
        dataService = new JsonDataService();
        logout = logoutCallback;

        // expose to UI
        Books = new ObservableCollection<Book>(libraryService.Books);
    }

    public void SaveData()
    {
    //create a object with current data
    var data = new LibraryData
    {
        Books = libraryService.Books,
        Loans = libraryService.Loans
    };
    
    //save in JSON
    dataService.SaveData(data);
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

    public Book? SelectedBook { get; set; }
    public void DeleteBook()
{
    if (SelectedBook != null)
    {
        libraryService.Books.Remove(SelectedBook);
        Books.Remove(SelectedBook);
        SaveData();
    }
}

    public void BorrowBook(Member member, Book book)
    {
        libraryService.BorrowBook(member,book);
        SaveData();
    }

    public void ReturnBook(Member member, Book book)
    {
        libraryService.ReturnBook(member, book);
        SaveData();
    }

    public void Logout()
    {
        logout();
    }

    

    }

    
