using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLibrary.Models;
using LocalLibrary.Services;

namespace LocalLibrary.ViewModels;

public partial class MemberViewModel : ViewModelBase, ILogoutHandler
{
	private readonly LibraryService _libraryService;
	private readonly Member _member;
	private readonly Action _saveData;
	private readonly Action _logout;

	public CatalogViewModel Catalog { get; }

	public ObservableCollection<Book> MyLoans { get; } = new();

	public string MemberName => _member.Username ?? "Member";

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(ReturnSelectedLoanCommand))]
	[NotifyCanExecuteChangedFor(nameof(RateSelectedLoanCommand))]
	private Book? selectedLoan;

	[ObservableProperty]
	private string statusMessage = string.Empty;

	[ObservableProperty]
	private string myRatingText = "My Rating: -/5.0";

	public MemberViewModel(LibraryService libraryService, Member member, Action saveData, Action logout)
	{
		_libraryService = libraryService;
		_member = member;
		_saveData = saveData;
		_logout = logout;

		Catalog = new CatalogViewModel(_libraryService, _member, _saveData, RefreshLoans);

		RefreshLoans();
	}

	[RelayCommand(CanExecute = nameof(CanReturnSelectedLoan))]
	private void ReturnSelectedLoan()
	{
		if (SelectedLoan is null)
		{
			StatusMessage = "Select a loan to return.";
			return;
		}

		var title = SelectedLoan.Title ?? "Untitled";
		var success = _libraryService.ReturnBook(_member, SelectedLoan);

		if (!success)
		{
			StatusMessage = "Book could not be returned.";
			return;
		}

		_saveData();
		RefreshLoans();
		Catalog.RefreshCatalogCommand.Execute(null);
		StatusMessage = $"Returned: {title}";
	}

	[RelayCommand(CanExecute = nameof(CanRateSelectedLoan))]
	private void RateSelectedLoan(string? ratingText)
	{
		if (SelectedLoan is null)
		{
			StatusMessage = "Select a loan to rate.";
			return;
		}

		if (!int.TryParse(ratingText, out var rating) || rating < 1 || rating > 5)
		{
			StatusMessage = "Rating must be between 1 and 5 stars.";
			return;
		}

		var success = _libraryService.RateBook(_member, SelectedLoan, rating);
		if (!success)
		{
			StatusMessage = "Could not save rating for this book.";
			return;
		}

		var selectedTitle = SelectedLoan.Title ?? "book";
		var selectedIsbn = SelectedLoan.ISBN;
		_saveData();
		RefreshLoans();
		SelectedLoan = MyLoans.FirstOrDefault(b =>
			string.Equals(b.ISBN, selectedIsbn, StringComparison.OrdinalIgnoreCase));
		Catalog.RefreshCatalogCommand.Execute(null);
		UpdateMyRatingText();
		StatusMessage = $"Rated {selectedTitle} with {rating} star(s).";
	}

	[RelayCommand]
	private void RefreshLoans()
	{
		MyLoans.Clear();

		var loans = _libraryService.GetBorrowedBooksForMember(_member);
		foreach (var loan in loans)
		{
			MyLoans.Add(loan);
		}

		UpdateMyRatingText();

		StatusMessage = $"You currently have {MyLoans.Count} borrowed book(s).";
	}

	private bool CanReturnSelectedLoan()
	{
		return SelectedLoan is not null;
	}

	private bool CanRateSelectedLoan()
	{
		return SelectedLoan is not null;
	}

	partial void OnSelectedLoanChanged(Book? value)
	{
		UpdateMyRatingText();
	}

	private void UpdateMyRatingText()
	{
		if (SelectedLoan is null)
		{
			MyRatingText = "My Rating: -/5.0";
			return;
		}

		var rating = SelectedLoan.GetMemberRating(_member.Username);
		MyRatingText = rating > 0
			? $"My Rating: {rating:F1}/5.0"
			: "My Rating: -/5.0";
	}

	public void Logout()
	{
		_logout();
	}
}
