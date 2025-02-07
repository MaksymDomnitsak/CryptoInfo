using CryptoInfo.Helpers.Commands;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Globalization;
using CryptoInfo.Models;

namespace CryptoInfo.ViewModels
{
    internal class CurrencyConvertViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient = new();
        private string? _selectedCrypto;
        private string? _selectedFiat;
        private string _amount = "";
        private string? _convertedAmount;
        private Dictionary<string, decimal> _priceDictionary = new();

        public ObservableCollection<CryptoSymbol> Cryptos { get; set; } = new();
        public ObservableCollection<string> FiatCurrencies { get; set; } = new();

        public string? SelectedCrypto
        {
            get => _selectedCrypto;
            set
            {
                Set(ref _selectedCrypto, value);     
                FetchCryptoPrice();
            }
        }

        public string? SelectedFiat
        {
            get => _selectedFiat;
            set
            {
                Set(ref _selectedFiat, value);
                ConvertCurrency();
            }
        }

        public string Amount
        {
            get => _amount;
            set
            {
                
                value = value.Replace('.', ',');
                if (value.IndexOf(',') != value.LastIndexOf(',')) return;
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    _amount = value;
                }
                else _amount = "";
                OnPropertyChanged(nameof(Amount));
                ConvertCurrency();
            }
        }

        public string? ConvertedAmount
        {
            get => _convertedAmount;
            set => Set(ref _convertedAmount, value);
        }

        public ICommand GoBackCommand { get; }

        public string previousPage = "";


        public CurrencyConvertViewModel()
        {
            GoBackCommand = new RelayCommand(GoBack, CanGoBack);
            LoadData(); 
        }

        private async Task LoadData()
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.coingecko.com/api/v3/simple/supported_vs_currencies"),
                    Headers =
                {
                    { "accept", "application/json" },
                    { "x-cg-demo-api-key", "CG-NX7yLkDvief1DLNfb1sMtmUk" },
                },
                };
                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var fiatList = JsonSerializer.Deserialize<List<string>>(body);
                    FiatCurrencies.Clear();
                    foreach (var fiat in fiatList)
                    {
                        FiatCurrencies.Add(fiat.ToUpper());
                    }
                }
            }
            catch (Exception ex) { }
        }

        public async void FetchCryptoPrice()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.coingecko.com/api/v3/coins/{SelectedCrypto}?localization=false&tickers=false&market_data=true&community_data=false&developer_data=false"),
                Headers =
                {
                    { "accept", "application/json" },
                    { "x-cg-demo-api-key", "CG-NX7yLkDvief1DLNfb1sMtmUk" },
                },
            };
            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                JObject cryptoInfo = JObject.Parse(body);
                var current_price = cryptoInfo["market_data"]?["current_price"] as JObject;
                _priceDictionary = current_price?
                .Properties().ToDictionary(
                    prop => prop.Name,
                    prop => prop.Value.Value<decimal>()
                ) ?? new Dictionary<string, decimal>();

                ConvertCurrency();
            }
        }

        private void ConvertCurrency()
        {
            if (_priceDictionary != null && SelectedFiat != null && _priceDictionary.ContainsKey(SelectedFiat.ToLower()))
            {
                ConvertedAmount = ((String.IsNullOrEmpty(Amount) ? 0 : Decimal.Parse(Amount)) * _priceDictionary[SelectedFiat.ToLower()]).ToString()+" "+SelectedFiat;
            }
            else
            {
                ConvertedAmount = "0";
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

    public class CryptoMarketData
    {
        public MarketData MarketData { get; set; }
    }

    public class MarketData
    {
        public Dictionary<string, decimal> CurrentPrice { get; set; }
    }
}
