using System;
using System.Collections.Generic;
namespace LocalLibrary.Models;


public class Book
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public bool IsBorrowed { get; set; }
}