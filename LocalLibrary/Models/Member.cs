using System;
using System.Collections.Generic;
namespace LocalLibrary.Models;

public class Member
{
    public string? Username { get; set; }
    public string? Password { get; set; }

    public List<Book> BorrowedBooks { get; set; } = new();
}