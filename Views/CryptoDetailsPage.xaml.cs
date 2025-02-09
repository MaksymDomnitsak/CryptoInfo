using System.Windows.Controls;
using CryptoInfo.ViewModels;


namespace CryptoInfo.Views
{
    /// <summary>
    /// Interaction logic for CryptoDetailsPage.xaml
    /// </summary>
    public partial class CryptoDetailsPage : Page
    {
        public CryptoDetailsPage(CryptoDetailsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
