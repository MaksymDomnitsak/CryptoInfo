using System.Windows.Controls;
using System.Windows.Input;
using CryptoInfo.ViewModels;

namespace CryptoInfo.Views
{
    /// <summary>
    /// Interaction logic for SearchCryptoPage.xaml
    /// </summary>
    public partial class SearchCryptoPage : Page
    {
        public SearchCryptoPage(SearchCryptoViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void ListBoxItem_LeftClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SearchCryptoViewModel viewModel && viewModel.SelectedCoin != null)
            {
                viewModel.OpenDetailsCommand.Execute(viewModel.SelectedCoin.Name);
            }
        }

        private void ListBoxItem_RightClick(object sender, MouseButtonEventArgs e)
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
                StatusBarText.Text = "Left-click on crypto for details, right-click for conversion of this crypto";
            }
        }

        private void ListBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (StatusBarText != null)
            {
                StatusBarText.Text = "";
            }
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (StatusBarText != null)
            {
                StatusBarText.Text = "Search cryptocurrencies by name or code";
            }
        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (StatusBarText != null)
            {
                StatusBarText.Text = "";
            }
        }




    }
}
