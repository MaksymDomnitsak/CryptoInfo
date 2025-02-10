using System.Windows;

namespace CryptoInfo.Helpers
{
    internal class GeneralMessageBoxForException
    {
        public static void Invoke(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(
                    message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            });
        }
    }
}
