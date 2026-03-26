using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLibrary.Models;
using LocalLibrary.Services;

namespace LocalLibrary.ViewModels;

public partial class CatalogViewModel : ViewModelBase
{
	private readonly LibraryService _libraryService;
	private readonly Member _member;
	private readonly Action _saveData;
	private readonly Action _refreshLoans;

	public ObservableCollection<Book> FilteredBooks { get; } = new();

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(BorrowSelectedBookCommand))]
	private Book? selectedBook;

	[ObservableProperty]
	private string searchText = string.Empty;

	[ObservableProperty]
	private string statusMessage = string.Empty;

	[ObservableProperty]
	private bool showEmptyState;

	[ObservableProperty]
	private string emptyStateMessage = "No books available in the catalog.";

	public CatalogViewModel(
		LibraryService libraryService,
		Member member,
		Action saveData,
		Action refreshLoans)
	{
		_libraryService = libraryService;
		_member = member;
		_saveData = saveData;
		_refreshLoans = refreshLoans;

		RefreshCatalog();
	}

	partial void OnSearchTextChanged(string value)
	{
		ApplySearchFilter();
	}

	[RelayCommand]
	private void RefreshCatalog()
	{
		ApplySearchFilter();
		StatusMessage = "Catalog refreshed.";
	}

	[RelayCommand]
	private void ClearSearch()
	{
		SearchText = string.Empty;
		ApplySearchFilter();
	}

	[RelayCommand(CanExecute = nameof(CanBorrowSelectedBook))]
	private void BorrowSelectedBook()
	{
		if (SelectedBook is null)
		{
			StatusMessage = "Select a book to borrow.";
			return;
		}

		var bookTitle = SelectedBook.Title ?? "Untitled";

		var success = _libraryService.BorrowBook(_member, SelectedBook);

		if (!success)
		{
			StatusMessage = "Book could not be borrowed.";
			return;
		}

		_saveData();
		_refreshLoans();

		var selectedIsbn = SelectedBook.ISBN;
		ApplySearchFilter();
		SelectedBook = FilteredBooks.FirstOrDefault(b =>
			string.Equals(b.ISBN, selectedIsbn, StringComparison.OrdinalIgnoreCase));

		StatusMessage = $"Borrowed: {bookTitle}";
	}

	private bool CanBorrowSelectedBook()
	{
		return SelectedBook is not null && !SelectedBook.IsBorrowed;
	}

	[RelayCommand]
	public void RateBook(int rating)
	{
		if (SelectedBook is null)
		{
			StatusMessage = "Select a book to rate.";
			return;
		}

		if (rating < 1 || rating > 5)
		{
			StatusMessage = "Rating must be between 1 and 5 stars.";
			return;
		}

		var success = _libraryService.RateBook(_member, SelectedBook, rating);

		if (!success)
		{
			StatusMessage = "Book cannot be rated. Only borrowed books can be rated.";
			return;
		}

		_saveData();
		StatusMessage = $"Rated {SelectedBook.Title} with {rating} star(s). Average: {SelectedBook.AverageRating}";
	}

	private void ApplySearchFilter()
	{
		FilteredBooks.Clear();

		var books = _libraryService.SearchAvailableBooks(SearchText);

		foreach (var book in books)
		{
			FilteredBooks.Add(book);
		}

		ShowEmptyState = FilteredBooks.Count == 0;
		EmptyStateMessage = string.IsNullOrWhiteSpace(SearchText)
			? "No books available in the catalog."
			: "No books match your search.";
	}
}
