﻿using CryptoInfo.Helpers.Commands;
using CryptoInfo.Models;
using CryptoInfo.Services;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CryptoInfo.ViewModels
{
    public class CryptoDetailsViewModel: BaseViewModel
    {
        private string? _name;
        private string? _symbol;
        private string _imageUrl = "https://w7.pngwing.com/pngs/607/198/png-transparent-bitcoin-faucet-computer-icons-cryptocurrency-bitcoin-text-logo-inside.png";
        private decimal _priceUsd;
        private string? _volumeUsd;
        private decimal _priceChange;

        private readonly ConnectToApiService _apiService;
        public ObservableCollection<MarketInfo> MarketList { get; set; } = new ObservableCollection<MarketInfo>();

        public CandlestickChartViewModel ChartViewModel { get; } = new CandlestickChartViewModel();

        public string? Name { get => _name; set => Set(ref _name, value); }
        public string? Symbol { get => _symbol; set => Set(ref _symbol, value); }
        public string ImageUrl { get => _imageUrl; set => Set(ref _imageUrl, value); }
        public decimal PriceUsd { get => _priceUsd; set => Set(ref _priceUsd, value); }
        public string? VolumeUsd { get => _volumeUsd; set => Set(ref _volumeUsd, value); }
        public decimal PriceChange { get => _priceChange; set => Set(ref _priceChange, value); }

        public ICommand OpenMarketCommand { get; }
        public ICommand GoBackCommand { get; }

        public string previousPage = "";

        public string CurrentId = "";

        public CryptoDetailsViewModel(ConnectToApiServiceFactory service)
        {
            _apiService = service.Create("CoinGecko");
            GoBackCommand = new RelayCommand(GoBack, CanGoBack);
            OpenMarketCommand = new RelayCommand(OpenMarket);
        }

        public async Task LoadCryptoData(string cryptoName) 
        {
            string uri = $"https://api.coingecko.com/api/v3/search?query={cryptoName}";
            try
            {
                var body = await _apiService.LoadDataFromApi(uri);
                var result = JsonSerializer.Deserialize<CryptoSearchResult>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result?.Coins != null && result?.Coins[0] != null)
                {
                    CurrentId = result?.Coins[0].Id;
                }
                uri = $"https://api.coingecko.com/api/v3/coins/{CurrentId}?localization=false&tickers=true&market_data=true&community_data=false&developer_data=false";
            
                string json = await _apiService.LoadDataFromApi(uri);
                ChartViewModel.SelectedCryptoId = CurrentId;
                JObject data = JObject.Parse(json);

                Name = data["name"]?.ToString();
                Symbol = data["symbol"]?.ToString();
                ImageUrl = data["image"]?["small"]?.ToString();
                PriceUsd = data["market_data"]?["current_price"]?["usd"]?.Value<decimal>() ?? 0;
                VolumeUsd = (data["market_data"]?["total_volume"]?["usd"]?.Value<string>() ?? "")+" USD";
                PriceChange = data["market_data"]?["price_change_percentage_24h"]?.Value<decimal>() ?? 0;

                MarketList.Clear();
                var tickers = data["tickers"]?.Take(10);
                if (tickers != null)
                {
                    foreach (var ticker in tickers)
                    {
                        MarketList.Add(new MarketInfo
                        {
                            Market = $"{ticker["market"]?["name"]} - {Math.Round(ticker["last"].Value<decimal>(),5)} USD",
                            TradeUrl = ticker["trade_url"]?.ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private void OpenMarket(object? url)
        {
            string? URL = url as string;
            if (!string.IsNullOrEmpty(URL))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = URL,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch { }
            }
        }

        private void GoBack(object? temp)
        {
            if (Application.Current.MainWindow is Window mainWindow && mainWindow.FindName(previousPage) != null)
            {
                Frame? frame = mainWindow.FindName(previousPage) as Frame;
                frame.GoBack();
            }
        }

        private bool CanGoBack(object? temp)
        {    
            return Application.Current.MainWindow is Window mainWindow && mainWindow.FindName(previousPage) != null;
        }
    }

}
