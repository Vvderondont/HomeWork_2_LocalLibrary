using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
namespace LocalLibrary.Models;


public class Book : INotifyPropertyChanged
{
    private string? title;
    private string? author;
    private string? isbn;
    private string? description;
    private bool isBorrowed;
    private List<BookRating> ratings = new();

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

    public List<BookRating> Ratings
    {
        get => ratings;
        set
        {
            if (ratings != value)
            {
                ratings = value;
                OnPropertyChanged();
            }
        }
    }

    public float AverageRating
    {
        get => ratings.Count > 0 
            ? MathF.Round((float)ratings.Average(r => r.Rating), 1) 
            : 0f;
    }

    public float GetMemberRating(string? memberName)
    {
        if (string.IsNullOrWhiteSpace(memberName))
        {
            return 0f;
        }

        var rating = ratings.FirstOrDefault(r =>
            string.Equals(r.MemberName, memberName, StringComparison.OrdinalIgnoreCase));

        return rating?.Rating ?? 0f;
    }

    public void NotifyRatingsChanged()
    {
        OnPropertyChanged(nameof(Ratings));
        OnPropertyChanged(nameof(AverageRating));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}