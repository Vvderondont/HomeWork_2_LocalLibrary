using System.Collections.Generic;
using Avalonia.Headless.XUnit;
using LocalLibrary.Models;
using LocalLibrary.Services;
using LocalLibrary.ViewModels;

namespace LocalLibrary.Tests.Headless;

public class DeleteBookHeadlessTests
{
    [AvaloniaFact]
    public void DeleteBook_FromSelectedBookFlow_RemovesBookFromAllCatalogs()
    {
        var targetBook = new Book
        {
            Title = "Delete Me",
            Author = "Test Author",
            ISBN = "444444444444",
            Description = "Book that should be deleted",
            IsBorrowed = false
        };

        var keepBook = new Book
        {
            Title = "Keep Me",
            Author = "Other Author",
            ISBN = "333333333333",
            Description = "Book that should remain",
            IsBorrowed = false
        };

        var data = new LibraryData
        {
            Books = new List<Book> { targetBook, keepBook },
            Members = new List<Member>
            {
                new() { Username = "member1", Password = "pass" },
                new() { Username = "member2", Password = "pass" }
            },
            Loans = new List<Loan>()
        };

        var service = new LibraryService(data);
        var memberOneVm = new MemberViewModel(service, data.Members[0], () => { }, () => { });
        var memberTwoVm = new MemberViewModel(service, data.Members[1], () => { }, () => { });
        var librarianVm = new LibrarianViewModel(data, () => { }, () => { });

        librarianVm.SelectedBook = targetBook;
        librarianVm.DeleteBook();
        memberOneVm.Catalog.RefreshCatalogCommand.Execute(null);
        memberTwoVm.Catalog.RefreshCatalogCommand.Execute(null);

        Assert.DoesNotContain(librarianVm.Books, b => b.ISBN == targetBook.ISBN);
        Assert.DoesNotContain(memberOneVm.Catalog.FilteredBooks, b => b.ISBN == targetBook.ISBN);
        Assert.DoesNotContain(memberTwoVm.Catalog.FilteredBooks, b => b.ISBN == targetBook.ISBN);
    }
}
