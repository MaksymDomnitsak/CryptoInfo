using CryptoInfo.Models;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using CryptoInfo.Helpers.Commands;
using CryptoInfo.Services;
using Microsoft.Extensions.DependencyInjection;
using CryptoInfo.Services.Interfaces;
using CryptoInfo.Helpers;


namespace CryptoInfo.ViewModels
{
    public class SearchCryptoViewModel : BaseViewModel
    {
        private readonly ConnectToApiService _apiService;

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

        public SearchCryptoViewModel(ConnectToApiServiceFactory service)
        {
            _apiService = service.Create("CoinGecko");
            if (Application.Current.MainWindow is not null)
            {
                var parentDataContext = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
                if (!string.IsNullOrWhiteSpace(parentDataContext.saveSearch))
                {
                    SearchQuery = parentDataContext.saveSearch;
                }
            }
            Coins = new ObservableCollection<CryptoCoinGecko>();
            OpenDetailsCommand = new RelayCommand(NavigateToDetails);
            OpenConverterCommand = new RelayCommand(NavigateToConverter);
            SearchCryptoAsync(SearchQuery);
        }

        public async Task SearchCryptoAsync(string query)
        {
            try
            {
                string uri = $"https://api.coingecko.com/api/v3/search?query={query}";
                var body = await _apiService.LoadDataFromApi(uri);
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
            catch (Exception ex) 
            {
                GeneralMessageBoxForException.Invoke(ex.Message);
            }
        }
        private void NavigateToDetails(object? cryptoName)
        {
            string? name = cryptoName as string;
            if (!string.IsNullOrEmpty(name))
            {
                var detailsPage = App.ServiceProvider.GetRequiredService<ICryptoDetailsPageFactory>().Create();
                var viewModel = (CryptoDetailsViewModel)detailsPage.DataContext;
                viewModel.PreviousPage = MethodBase.GetCurrentMethod().DeclaringType.Name.Replace("ViewModel", "");
                viewModel?.LoadCryptoData(name);
                var parentDataContext = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
                parentDataContext.saveSearch = SearchQuery;
                Frame? frame = NavigationHelper.FindNavigationFrame(viewModel.PreviousPage);               
                frame?.Navigate(detailsPage);
            }
        }

        private void NavigateToConverter(object? cryptoName)
        {
            string? name = cryptoName as string;
            if (!string.IsNullOrEmpty(name))
            {
                var converterPage = App.ServiceProvider.GetRequiredService<ICurrencyConvertPageFactory>().Create();
                var viewModel = (CurrencyConvertViewModel)converterPage.DataContext;
                viewModel.SelectedCrypto = name;
                viewModel.PreviousPage = MethodBase.GetCurrentMethod().DeclaringType.Name.Replace("ViewModel", "");
                Frame? frame = NavigationHelper.FindNavigationFrame(viewModel.PreviousPage);
                var parentDataContext = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
                parentDataContext.saveSearch = SearchQuery;
                frame?.Navigate(converterPage);
            }
        }
    }
}
