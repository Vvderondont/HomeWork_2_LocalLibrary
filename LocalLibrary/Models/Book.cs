using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace LocalLibrary.Models;


public class Book : INotifyPropertyChanged
{
    private string? title;
    private string? author;
    private string? isbn;
    private string? description;
    private bool isBorrowed;

    public string? Title
    {
        get => title;
        set
        {
            if (title != value)
            {
                title = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Author
    {
        get => author;
        set
        {
            if (author != value)
            {
                author = value;
                OnPropertyChanged();
            }
        }
    }

    public string? ISBN
    {
        get => isbn;
        set
        {
            if (isbn != value)
            {
                isbn = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Description
    {
        get => description;
        set
        {
            if (description != value)
            {
                description = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsBorrowed
    {
        get => isBorrowed;
        set
        {
            if (isBorrowed != value)
            {
                isBorrowed = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}