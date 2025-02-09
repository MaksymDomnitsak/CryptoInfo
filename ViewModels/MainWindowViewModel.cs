using System.Windows.Controls;
using System.Windows.Input;
using CryptoInfo.Helpers.Commands;
using CryptoInfo.Views;

namespace CryptoInfo.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public Page TopOfCryptosPage { get; }

        public Page SearchCryptoPage { get; }

        private string _CurrentTheme = "Light";

        private string _CurrentLanguage = "en";

        public string saveSearch = "";

        public ICommand ToggleThemeCommand { get; set; }
        public ICommand ChangeLanguageCommand { get; set; }

        public MainWindowViewModel(TopOfCryptosPage page,SearchCryptoPage searchPage)
        {
            ToggleThemeCommand = new RelayCommand(ToggleTheme, CanToggleTheme);
            ChangeLanguageCommand = new RelayCommand(ChangeLanguage, CanChangeLanguage);
            TopOfCryptosPage = page;
            SearchCryptoPage = searchPage;
        }

        private void ToggleTheme(object? theme)
        {
            _CurrentTheme = theme as string;

        }

        private bool CanToggleTheme(object? theme)
        {
            if(_CurrentTheme == (string?)theme) return false;
            return true;
        }

        private void ChangeLanguage(object? languageCode)
        {
            _CurrentLanguage = languageCode as string;

        }

        private bool CanChangeLanguage(object? languageCode)
        {
            if (_CurrentLanguage == (string?)languageCode) return false;
            return true;
        }

    }
}
