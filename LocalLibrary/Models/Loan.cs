using System;
namespace LocalLibrary.Models;

public class Loan
{
    public string? BookTitle { get; set; }
    public string? BookISBN { get; set; }
    public string? MemberName { get; set; }
    public DateTime BorrowDate { get; set; }
}