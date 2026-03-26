using System.Collections.Generic;
using LocalLibrary.Models;
using LocalLibrary.Services;

namespace LocalLibrary.Tests.Unit;

public class BorrowReturnTests
{
    [Fact]
    public void BorrowBook_WhenAvailable_CreatesLoanAndMarksBookAsBorrowed()
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

        var result = service.BorrowBook(member, book);

        Assert.True(result);
        Assert.True(book.IsBorrowed);
        Assert.Contains(member.BorrowedBooks, b => b.ISBN == book.ISBN);
        Assert.Contains(data.Loans, l => l.BookISBN == book.ISBN && l.MemberName == member.Username);
    }

    [Fact]
    public void ReturnBook_WhenLoanExists_RemovesLoanAndMarksBookAsAvailable()
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

        var result = service.ReturnBook(member, book);

        Assert.True(result);
        Assert.False(book.IsBorrowed);
        Assert.DoesNotContain(member.BorrowedBooks, b => b.ISBN == book.ISBN);
        Assert.DoesNotContain(data.Loans, l => l.BookISBN == book.ISBN && l.MemberName == member.Username);
    }
}
