using System.Collections.Generic;
using Avalonia.Headless.XUnit;
using LocalLibrary.Models;
using LocalLibrary.Services;
using LocalLibrary.ViewModels;

namespace LocalLibrary.Tests.Headless;

public class BorrowReturnHeadlessTests
{
    [AvaloniaFact]
    public void BorrowSelectedBookCommand_AddsLoanAndUpdatesCollections()
    {
        var member = new Member
        {
            Username = "member1",
            Password = "pass",
            BorrowedBooks = new List<Book>()
        };

        var book = new Book
        {
            Title = "Create",
            Author = "Trump",
            ISBN = "1111111111111",
            IsBorrowed = false
        };

        var data = new LibraryData
        {
            Members = new List<Member> { member },
            Books = new List<Book> { book },
            Loans = new List<Loan>()
        };

        var service = new LibraryService(data);
        var vm = new MemberViewModel(service, member, () => { }, () => { });

        vm.Catalog.SelectedBook = vm.Catalog.FilteredBooks[0];
        vm.Catalog.BorrowSelectedBookCommand.Execute(null);

        Assert.True(book.IsBorrowed);
        Assert.Single(vm.MyLoans);
        Assert.Equal("Borrowed: Create", vm.Catalog.StatusMessage);
    }

    [AvaloniaFact]
    public void ReturnSelectedLoanCommand_RemovesLoanAndMakesBookAvailable()
    {
        var member = new Member
        {
            Username = "member1",
            Password = "pass",
            BorrowedBooks = new List<Book>()
        };

        var book = new Book
        {
            Title = "Create",
            Author = "Trump",
            ISBN = "1111111111111",
            IsBorrowed = false
        };

        var data = new LibraryData
        {
            Members = new List<Member> { member },
            Books = new List<Book> { book },
            Loans = new List<Loan>()
        };

        var service = new LibraryService(data);
        service.BorrowBook(member, book);

        var vm = new MemberViewModel(service, member, () => { }, () => { });
        vm.SelectedLoan = vm.MyLoans[0];
        vm.ReturnSelectedLoanCommand.Execute(null);

        Assert.False(book.IsBorrowed);
        Assert.Empty(vm.MyLoans);
        Assert.Equal("Returned: Create", vm.StatusMessage);
    }
}
