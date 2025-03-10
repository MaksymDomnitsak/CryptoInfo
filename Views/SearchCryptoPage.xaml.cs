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
  }
}
