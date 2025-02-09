using System.Windows.Controls;
using CryptoInfo.ViewModels;


namespace CryptoInfo.Views
{
    /// <summary>
    /// Interaction logic for CurrencyConvertPage.xaml
    /// </summary>
    public partial class CurrencyConvertPage : Page
    {
        public CurrencyConvertPage(CurrencyConvertViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
