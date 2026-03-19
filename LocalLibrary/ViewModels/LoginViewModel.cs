using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LocalLibrary.ViewModels
{
public partial class LoginViewModel : ViewModelBase
{
  [ObservableProperty]private string _errorMessage="";
  [RelayCommand]

  private async Task Login()
  {

  }  
}
}