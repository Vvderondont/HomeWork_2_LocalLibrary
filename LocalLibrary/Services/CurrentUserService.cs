namespace LocalLibrary.Services;

public class CurrentUserService
{
    public string? LoggedInUsername { get; set; }
    public string? UserRole { get; set; } // "Member" or "Librarian"
    public object? UserDetails { get; set; } // Member object if member, null if librarian
}
