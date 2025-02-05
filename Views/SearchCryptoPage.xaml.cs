using System.Windows.Controls;
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
    }
}
