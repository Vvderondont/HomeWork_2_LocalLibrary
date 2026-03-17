using LocalLibrary.Services;
using LocalLibrary.Models;
namespace LocalLibrary.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    private readonly JsonDataService dataService;

    public LibraryData LibraryData { get; set; }

    public MainWindowViewModel()
    {
        dataService = new JsonDataService();

    
        LibraryData = dataService.LoadData();
    }

    public void SaveData()
    {
        dataService.SaveData(LibraryData);
    }
}