using System.Windows.Input;
using CryptoInfo.Helpers.Commands;

namespace CryptoInfo.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private string _CurrentTheme = "Light";

        private string _CurrentLanguage = "en";

        public ICommand ToggleThemeCommand { get; set; }
        public ICommand ChangeLanguageCommand { get; set; }

        public MainWindowViewModel()
        {
            ToggleThemeCommand = new RelayCommand(ToggleTheme, CanToggleTheme);
            ChangeLanguageCommand = new RelayCommand(ChangeLanguage, CanChangeLanguage);
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
