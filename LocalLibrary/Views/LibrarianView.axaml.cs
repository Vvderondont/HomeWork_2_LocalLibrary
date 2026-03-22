using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using LocalLibrary.Models;
using LocalLibrary.ViewModels;
using System.Linq;

namespace LocalLibrary.Views;

public partial class LibrarianView : UserControl
{
    public LibrarianView()
    {
        InitializeComponent();
    }

    private void AddBook_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not LibrarianViewModel vm)
        {
            SetStatus("Could not access the data context.", true);
            return;
        }

        var title = TitleTextBox.Text?.Trim();
        var author = AuthorTextBox.Text?.Trim();
        var isbn = IsbnTextBox.Text?.Trim();

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(isbn))
        {
            SetStatus("Please complete Title, Author, and ISBN to add a book.", true);
            return;
        }

        var exists = vm.Books.Any(b => string.Equals(b.ISBN, isbn, System.StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            SetStatus("A book with that ISBN already exists.", true);
            return;
        }

        vm.AddBook(new Book
        {
            Title = title,
            Author = author,
            ISBN = isbn,
            IsBorrowed = false
        });

        TitleTextBox.Text = string.Empty;
        AuthorTextBox.Text = string.Empty;
        IsbnTextBox.Text = string.Empty;

        SetStatus("Book added successfully.", false);
    }

    private void DeleteBook_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not LibrarianViewModel vm)
        {
            SetStatus("Could not access the data context.", true);
            return;
        }

        if (BooksListBox.SelectedItem is Book selectedBook)
        {
            vm.DeleteBook(selectedBook);
            SetStatus("Book deleted successfully.", false);
            return;
        }

        SetStatus("Select a book to delete.", true);
    }

    private void SetStatus(string message, bool isError)
    {
        StatusTextBlock.Text = message;
        StatusTextBlock.Foreground = isError ? Brushes.IndianRed : Brushes.ForestGreen;
    }
    private void EditBook_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is LibrarianViewModel vm)
        {
            if (vm.SelectedBook != null)
            {
                vm.EditBook();
                SetStatus("Book changes saved successfully.", false);
            }
            else
            {
                SetStatus("Please select a book to edit.", true);
            }
        }
    }

}