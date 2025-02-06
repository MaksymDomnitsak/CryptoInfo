using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using CryptoInfo.ViewModels;

namespace CryptoInfo.Views
{
    /// <summary>
    /// Логика взаимодействия для SearchCryptoPage.xaml
    /// </summary>
    public partial class SearchCryptoPage : Page
    {
        public SearchCryptoPage()
        {
            InitializeComponent();
        }

        private void ListBoxItem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is SearchCryptoViewModel viewModel && viewModel.SelectedCoin != null)
            {
                viewModel.OpenDetailsCommand.Execute(viewModel.SelectedCoin.Id);
            }
        }

        private void ListBoxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is SearchCryptoViewModel viewModel && viewModel.SelectedCoin != null)
            {
                viewModel.OpenConverterCommand.Execute(viewModel.SelectedCoin.Id);
            }
        }
        private void ListBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (StatusBarText != null)
            {
                StatusBarText.Text = "Натисніть для деталей, двічі клацніть для конвертації цієї валюти";
                //StatusBarText.Text = "Click for details, double-click for conversion of this crypto";
            }
        }

        private void ListBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (StatusBarText != null)
            {
                StatusBarText.Text = "Ready";
            }
        }


    }
}
