using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using LocalLibrary.Services;
using LocalLibrary.ViewModels;
using LocalLibrary.Views;

namespace LocalLibrary;

public partial class App : Application
{
    private MainWindowViewModel? _mainViewModel;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            
            var dataService = new JsonDataService();
            var data = dataService.LoadData();

            desktop.MainWindow = new MainWindow
            {
                Content = new LibrarianView
                {
                    DataContext = new LibrarianViewModel(data)
                }
            };
            
            desktop.Exit += OnExit;//save automatetly
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        //_mainViewModel?.SaveData();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}