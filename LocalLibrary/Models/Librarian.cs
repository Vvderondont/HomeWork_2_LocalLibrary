using System;
using System.Collections.Generic;
namespace LocalLibrary.Models;

public class LibraryData
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public List<Book> Books { get; set; } = new();

    public List<Member> Members { get; set; } = new();

    public List<Loan> Loans { get; set; } = new();
}