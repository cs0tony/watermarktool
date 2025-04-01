using H.NotifyIcon;
using System.Configuration;
using System.Data;
using System.Windows;

namespace watermarktool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon? notifyIcon;

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            notifyIcon.ForceCreate();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            notifyIcon?.Dispose(); //the icon would clean up automatically, but this is cleaner
        }
    }

}
