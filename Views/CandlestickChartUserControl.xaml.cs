using System.Windows.Controls;
using CryptoInfo.ViewModels;

namespace CryptoInfo.Views
{
    /// <summary>
    /// Interaction logic for CandlestickChartUserControl.xaml
    /// </summary>
    public partial class CandlestickChartUserControl : UserControl
    {
        public CandlestickChartUserControl(CandlestickChartViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
