using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLibrary.Models;
using LocalLibrary.Services;

namespace LocalLibrary.ViewModels;

public partial class MemberViewModel : ViewModelBase
{
	private readonly LibraryService _libraryService;
	private readonly Member _member;
	private readonly Action _saveData;

	public CatalogViewModel Catalog { get; }

	public ObservableCollection<Book> MyLoans { get; } = new();

	public string MemberName => _member.Username ?? "Member";

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(ReturnSelectedLoanCommand))]
	private Book? selectedLoan;

	[ObservableProperty]
	private string statusMessage = string.Empty;

	public MemberViewModel(LibraryService libraryService, Member member, Action saveData)
	{
		_libraryService = libraryService;
		_member = member;
		_saveData = saveData;

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

	[RelayCommand]
	private void RefreshLoans()
	{
		MyLoans.Clear();

		var loans = _libraryService.GetBorrowedBooksForMember(_member);
		foreach (var loan in loans)
		{
			MyLoans.Add(loan);
		}

		StatusMessage = $"You currently have {MyLoans.Count} borrowed book(s).";
	}

	private bool CanReturnSelectedLoan()
	{
		return SelectedLoan is not null;
	}
}
