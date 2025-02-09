using System.Windows.Controls;
using System.Windows;

namespace CryptoInfo.Helpers.Commands
{
    internal class NavigateCommands
    {
        public static void GoBack(object? previous)
        {

            if (Application.Current.MainWindow is Window mainWindow && mainWindow.FindName(previous as string) != null)
            {
                Frame? frame = mainWindow.FindName(previous as string) as Frame;
                frame.GoBack();
            }
        }

        public static bool CanGoBack(object? previous)
        {
            return Application.Current.MainWindow is Window mainWindow && mainWindow.FindName(previous as string) != null;
        }
    }
}
