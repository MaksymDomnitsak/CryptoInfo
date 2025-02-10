using System.Windows.Controls;
using CryptoInfo.Views;

namespace CryptoInfo.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public Page TopOfCryptosPage { get; }

        public Page SearchCryptoPage { get; }

        public string saveSearch = "";

        public MainWindowViewModel(TopOfCryptosPage page,SearchCryptoPage searchPage)
        {
            TopOfCryptosPage = page;
            SearchCryptoPage = searchPage;
        }
    }
}
