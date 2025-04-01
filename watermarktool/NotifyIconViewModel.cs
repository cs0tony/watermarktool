using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace watermarktool
{
    public partial class NotifyIconViewModel : ObservableObject
    {
        [RelayCommand]
        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
