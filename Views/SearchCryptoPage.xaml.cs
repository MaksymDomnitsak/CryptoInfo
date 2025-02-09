using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using CryptoInfo.ViewModels;
using Microsoft.Extensions.DependencyInjection;

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

        private void ListBoxItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SearchCryptoViewModel viewModel && viewModel.SelectedCoin != null)
            {
                viewModel.OpenDetailsCommand.Execute(viewModel.SelectedCoin.Name);
            }
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                StatusBarText.Text = "Натисніть ЛКМ для деталей, ПКМ для конвертації вибраної валюти";
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
