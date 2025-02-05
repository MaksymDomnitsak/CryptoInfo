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
        private string? _searchQuery;
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
            }
        }

        private CryptoCoinGecko? _selectedCoin;
        public CryptoCoinGecko? SelectedCoin
        {
            get => _selectedCoin;
            set => Set(ref _selectedCoin, value);
        }

        public ICommand OpenDetailsCommand { get; }

        public ObservableCollection<CryptoCoinGecko> Coins { get; set; }

        public SearchCryptoViewModel()
        {
            Coins = new ObservableCollection<CryptoCoinGecko>();
            OpenDetailsCommand = new RelayCommand(NavigateToDetails); // change name of command
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
                    frame?.Navigate(detailsPage);
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
