using System.Windows;

namespace CryptoInfo.Helpers
{
  internal class GeneralMessageBoxForException
  {
    public static void Invoke(string message, string caption = "Error")
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        MessageBox.Show(
                  message,
                  caption,
                  MessageBoxButton.OK,
                  MessageBoxImage.Warning
              );
      });
    }
  }
}
