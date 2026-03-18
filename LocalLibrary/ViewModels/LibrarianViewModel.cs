using LocalLibrary.Models;
using LocalLibrary.Services;
using System.Collections.Generic;
namespace LocalLibrary.ViewModels;

public class LibrarianViewModel : ViewModelBase
{
    private readonly LibraryService libraryService;
    private readonly JsonDataService dataService;

    public List<Book> Books { get; set; }

    public LibrarianViewModel(LibraryData data)
    {   
        //services
        libraryService = new LibraryService(data);
        dataService = new JsonDataService();

        // expose to UI
        Books = libraryService.Books;
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
        SaveData();

    }

    public void DelateBook(Book book)
    {
        libraryService.Books.Remove(book);
        SaveData();
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

    }

    
