using System.Windows.Controls;
using CryptoInfo.Views;

namespace CryptoInfo.ViewModels
{
  public class MainWindowViewModel(TopOfCryptosPage page, SearchCryptoPage searchPage) : BaseViewModel
    {
    public string saveSearch = "";
    public Page TopOfCryptosPage { get; } = page;
    public Page SearchCryptoPage { get; } = searchPage;

  }
}
