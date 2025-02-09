using System.Windows.Controls;
using CryptoInfo.ViewModels;
using Microsoft.Extensions.DependencyInjection;


namespace CryptoInfo.Views
{
    /// <summary>
    /// Interaction logic for TopOfCryptosPage.xaml
    /// </summary>
    public partial class TopOfCryptosPage : Page
    {
        public TopOfCryptosPage(TopOfCryptosViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
