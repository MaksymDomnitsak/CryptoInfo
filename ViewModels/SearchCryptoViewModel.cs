using CryptoInfo.Models;
using CryptoInfo.Views;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using CryptoInfo.Helpers.Commands;


namespace CryptoInfo.ViewModels
{
    internal class SearchCryptoViewModel : BaseViewModel
    {
        private string? _searchQuery = "BTC";
        public string? SearchQuery
        {
            get => _searchQuery;
            set
            {
                Set(ref  _searchQuery, value);     
                if (!string.IsNullOrWhiteSpace(_searchQuery))
                {
                    _ = SearchCryptoAsync(_searchQuery);
                }
                else
                {
                    Coins.Clear();
                }
            }
        }

        private CryptoCoinGecko? _selectedCoin;
        public CryptoCoinGecko? SelectedCoin
        {
            get => _selectedCoin;
            set => Set(ref _selectedCoin, value);
        }

        public ICommand OpenDetailsCommand { get; }

        public ICommand OpenConverterCommand { get; }

        public ObservableCollection<CryptoCoinGecko> Coins { get; set; }

        public SearchCryptoViewModel()
        {
            if (!string.IsNullOrWhiteSpace((Application.Current.MainWindow.DataContext as MainWindowViewModel).saveSearch)){
                SearchQuery = (Application.Current.MainWindow.DataContext as MainWindowViewModel).saveSearch;
            }
            Coins = new ObservableCollection<CryptoCoinGecko>();
            OpenDetailsCommand = new RelayCommand(NavigateToDetails); // change name of command
            OpenConverterCommand = new RelayCommand(NavigateToConverter);
            SearchCryptoAsync(SearchQuery);
        }

        public async Task SearchCryptoAsync(string query)
        {
            HttpClient _httpClient = new();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.coingecko.com/api/v3/search?query={query}"),
                Headers =
                {
                    { "accept", "application/json" },
                    { "x-cg-demo-api-key", "CG-NX7yLkDvief1DLNfb1sMtmUk" },
                },
            };
            try
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<CryptoSearchResult>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result?.Coins != null)
                    {
                        Coins.Clear();
                        foreach (var coin in result.Coins)
                        {
                            coin.Symbol = "(" + coin.Symbol + ")";
                            Coins.Add(coin);
                        }
                    }
                }
            }
            catch { }
        }
        private void NavigateToDetails(object? cryptoName)
        {
            string? name = cryptoName as string;
            if (!string.IsNullOrEmpty(name))
            {
                var detailsPage = new CryptoDetailsPage();
                var viewModel = (CryptoDetailsViewModel)detailsPage.DataContext;
                viewModel.previousPage = MethodBase.GetCurrentMethod().DeclaringType.Name.Replace("ViewModel", "");
                viewModel?.LoadCryptoData(name);
                if (Application.Current.MainWindow is not NavigationWindow navWindow)
                {
                    Frame? frame = FindNavigationFrame(viewModel.previousPage);
                    (Application.Current.MainWindow.DataContext as MainWindowViewModel).saveSearch = SearchQuery;
                    frame?.Navigate(detailsPage);
                }
            }
        }

        private void NavigateToConverter(object? cryptoName)
        {
            string? name = cryptoName as string;
            if (!string.IsNullOrEmpty(name))
            {
                var converterPage = new CurrencyConvertPage();
                var viewModel = (CurrencyConvertViewModel)converterPage.DataContext;
                viewModel.SelectedCrypto = name;
                viewModel.previousPage = MethodBase.GetCurrentMethod().DeclaringType.Name.Replace("ViewModel", "");
                if (Application.Current.MainWindow is not NavigationWindow navWindow)
                {
                    Frame? frame = FindNavigationFrame(viewModel.previousPage);
                    (Application.Current.MainWindow.DataContext as MainWindowViewModel).saveSearch = SearchQuery;
                    frame?.Navigate(converterPage);
                }
            }
        }

        private Frame? FindNavigationFrame(string frameName)
        {
            if (Application.Current.MainWindow is Window mainWindow)
            {
                return mainWindow.FindName(frameName) as Frame;
            }
            return null;
        }
    }

 

    internal class CryptoSearchResult
    {
        public List<CryptoCoinGecko>? Coins { get; set; }
    }
}
