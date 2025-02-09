﻿using CryptoInfo.Models;
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
                if (!string.IsNullOrWhiteSpace((Application.Current.MainWindow.DataContext as MainWindowViewModel).saveSearch))
                {
                    SearchQuery = (Application.Current.MainWindow.DataContext as MainWindowViewModel).saveSearch;
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
            catch { }
        }
        private void NavigateToDetails(object? cryptoName)
        {
            string? name = cryptoName as string;
            if (!string.IsNullOrEmpty(name))
            {
                var factory = App.ServiceProvider.GetRequiredService<ICryptoDetailsPageFactory>();
                var detailsPage = factory.Create();
                var viewModel = (CryptoDetailsViewModel)detailsPage.DataContext;
                viewModel.previousPage = MethodBase.GetCurrentMethod().DeclaringType.Name.Replace("ViewModel", "");
                viewModel?.LoadCryptoData(name);
                Frame? frame = FindNavigationFrame(viewModel.previousPage);
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).saveSearch = SearchQuery;
                frame?.Navigate(detailsPage);
            }
        }

        private void NavigateToConverter(object? cryptoName)
        {
            string? name = cryptoName as string;
            if (!string.IsNullOrEmpty(name))
            {
                var factory = App.ServiceProvider.GetRequiredService<ICurrencyConvertPageFactory>();
                var converterPage = factory.Create();
                var viewModel = (CurrencyConvertViewModel)converterPage.DataContext;
                viewModel.SelectedCrypto = name;
                viewModel.previousPage = MethodBase.GetCurrentMethod().DeclaringType.Name.Replace("ViewModel", "");
                Frame? frame = FindNavigationFrame(viewModel.previousPage);
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).saveSearch = SearchQuery;
                frame?.Navigate(converterPage);
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
}
