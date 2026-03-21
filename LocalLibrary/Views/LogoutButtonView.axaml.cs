using Avalonia.Controls;
using Avalonia.Interactivity;
using LocalLibrary.ViewModels;

namespace LocalLibrary.Views;

public partial class LogoutButtonView : UserControl
{
	public LogoutButtonView()
	{
		InitializeComponent();
	}

	private void Logout_Click(object? sender, RoutedEventArgs e)
	{
		if (DataContext is ILogoutHandler logoutHandler)
		{
			logoutHandler.Logout();
		}
	}
}
