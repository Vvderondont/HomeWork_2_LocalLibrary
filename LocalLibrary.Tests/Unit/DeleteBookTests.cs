using System.Collections.Generic;
using LocalLibrary.Models;
using LocalLibrary.Services;
using LocalLibrary.ViewModels;

namespace LocalLibrary.Tests.Unit;

public class DeleteBookTests
{
    [Fact]
    public void DeleteBook_RemovesBookFromAllCatalogViews()
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
            ISBN = "3333333333333",
            Description = "Book that should remain",
            IsBorrowed = false
        };

        var data = new LibraryData
        {
            Books = new List<Book> { targetBook, keepBook },
            Members = new List<Member>
            {
                new()
                {
                    Username = "member1",
                    Password = "pass"
                },
                new()
                {
                    Username = "member2",
                    Password = "pass"
                }
            },
            Loans = new List<Loan>()
        };

        var sharedLibraryService = new LibraryService(data);
        var firstMemberViewModel = new MemberViewModel(sharedLibraryService, data.Members[0], () => { }, () => { });
        var secondMemberViewModel = new MemberViewModel(sharedLibraryService, data.Members[1], () => { }, () => { });
        var librarianViewModel = new LibrarianViewModel(data, () => { }, () => { });

        Assert.Contains(librarianViewModel.Books, b => b.ISBN == targetBook.ISBN);
        Assert.Contains(firstMemberViewModel.Catalog.FilteredBooks, b => b.ISBN == targetBook.ISBN);
        Assert.Contains(secondMemberViewModel.Catalog.FilteredBooks, b => b.ISBN == targetBook.ISBN);

        librarianViewModel.DeleteBook(targetBook);
        firstMemberViewModel.Catalog.RefreshCatalogCommand.Execute(null);
        secondMemberViewModel.Catalog.RefreshCatalogCommand.Execute(null);

        Assert.DoesNotContain(librarianViewModel.Books, b => b.ISBN == targetBook.ISBN);
        Assert.DoesNotContain(sharedLibraryService.Books, b => b.ISBN == targetBook.ISBN);
        Assert.DoesNotContain(firstMemberViewModel.Catalog.FilteredBooks, b => b.ISBN == targetBook.ISBN);
        Assert.DoesNotContain(secondMemberViewModel.Catalog.FilteredBooks, b => b.ISBN == targetBook.ISBN);
        Assert.Contains(firstMemberViewModel.Catalog.FilteredBooks, b => b.ISBN == keepBook.ISBN);
        Assert.Contains(secondMemberViewModel.Catalog.FilteredBooks, b => b.ISBN == keepBook.ISBN);
    }
}
