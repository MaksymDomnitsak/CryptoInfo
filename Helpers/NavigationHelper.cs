using System.Windows.Controls;
using System.Windows;

namespace CryptoInfo.Helpers
{
  internal class NavigationHelper
  {
    public static Frame? FindNavigationFrame(string frameName)
    {
      if (Application.Current.MainWindow is Window mainWindow)
      {
        return mainWindow.FindName(frameName) as Frame;
      }
      return null;
    }
  }
}
