using System;
using System.Collections.Generic;
using System.Linq;
using LocalLibrary.Models;

namespace LocalLibrary.Services;

public class LibraryService
{
	private readonly LibraryData _libraryData;

	public LibraryService(LibraryData libraryData)
	{
		_libraryData = libraryData;
	}

	public IReadOnlyList<Book> GetAvailableBooks()
	{
		return _libraryData.Books
			.Where(b => !b.IsBorrowed)
			.ToList();
	}

	public IReadOnlyList<Book> SearchAvailableBooks(string? searchTerm)
	{
		if (string.IsNullOrWhiteSpace(searchTerm))
		{
			return GetAvailableBooks();
		}

		var term = searchTerm.Trim();

		return _libraryData.Books
			.Where(b => !b.IsBorrowed)
			.Where(b =>
				(b.Title?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
				(b.Author?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
				(b.ISBN?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
			.ToList();
	}

	public IReadOnlyList<Book> GetBorrowedBooksForMember(Member member)
	{
		var memberName = member.Username ?? string.Empty;

		var borrowedBookIsbns = _libraryData.Loans
			.Where(l => string.Equals(l.MemberName, memberName, StringComparison.OrdinalIgnoreCase))
			.Select(l => l.BookISBN)
			.Where(isbn => !string.IsNullOrWhiteSpace(isbn))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		return _libraryData.Books
			.Where(b => !string.IsNullOrWhiteSpace(b.ISBN) && borrowedBookIsbns.Contains(b.ISBN))
			.ToList();
	}

	public bool BorrowBook(Member member, Book book)
	{
		if (book.IsBorrowed || string.IsNullOrWhiteSpace(book.ISBN))
		{
			return false;
		}

		var memberName = member.Username ?? string.Empty;

		var existingLoan = _libraryData.Loans.Any(l =>
			string.Equals(l.BookISBN, book.ISBN, StringComparison.OrdinalIgnoreCase) &&
			string.Equals(l.MemberName, memberName, StringComparison.OrdinalIgnoreCase));

		if (existingLoan)
		{
			return false;
		}

		book.IsBorrowed = true;

		if (!member.BorrowedBooks.Any(b => string.Equals(b.ISBN, book.ISBN, StringComparison.OrdinalIgnoreCase)))
		{
			member.BorrowedBooks.Add(book);
		}

		_libraryData.Loans.Add(new Loan
		{
			BookISBN = book.ISBN,
			MemberName = memberName,
			BorrowDate = DateTime.UtcNow
		});

		return true;
	}

	public bool ReturnBook(Member member, Book book)
	{
		if (!book.IsBorrowed || string.IsNullOrWhiteSpace(book.ISBN))
		{
			return false;
		}

		var memberName = member.Username ?? string.Empty;

		var loanToRemove = _libraryData.Loans.FirstOrDefault(l =>
			string.Equals(l.BookISBN, book.ISBN, StringComparison.OrdinalIgnoreCase) &&
			string.Equals(l.MemberName, memberName, StringComparison.OrdinalIgnoreCase));

		if (loanToRemove is null)
		{
			return false;
		}

		book.IsBorrowed = false;

		var borrowedBook = member.BorrowedBooks.FirstOrDefault(b =>
			string.Equals(b.ISBN, book.ISBN, StringComparison.OrdinalIgnoreCase));

		if (borrowedBook is not null)
		{
			member.BorrowedBooks.Remove(borrowedBook);
		}

		_libraryData.Loans.Remove(loanToRemove);

		return true;
	}
}
