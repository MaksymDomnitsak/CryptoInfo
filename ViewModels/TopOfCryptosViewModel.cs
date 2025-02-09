using System.Collections.ObjectModel;
using System.Net.Http;
using CryptoInfo.Models;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using CryptoInfo.Views;
using CryptoInfo.Helpers.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using CryptoInfo.Services;
using Microsoft.Extensions.DependencyInjection;
using CryptoInfo.Services.Interfaces;

namespace CryptoInfo.ViewModels
{
    public class TopOfCryptosViewModel : BaseViewModel
    {
        private readonly ConnectToApiService _apiService;
        private ObservableCollection<CryptoCoinCap> _cryptocurrencies;
        public ObservableCollection<int> CryptoLimits { get; } = new ObservableCollection<int> { 10, 20 };
        public ObservableCollection<CryptoCoinCap> Cryptocurrencies
        {
            get => _cryptocurrencies;
            set => Set(ref _cryptocurrencies, value);
        }

        private int _cryptoViewLimit = 10;

        public int CryptoViewLimit
        {
            get => _cryptoViewLimit;
            set
            {
                Set(ref _cryptoViewLimit, value);
                LoadCryptocurrenciesAsync();
            }
        }

        public ICommand NavigateToDetailsCommand { get; }

        public TopOfCryptosViewModel(ConnectToApiServiceFactory service) 
        {
            _apiService = service.Create();
            NavigateToDetailsCommand = new RelayCommand(NavigateToDetails);
            Cryptocurrencies = new ObservableCollection<CryptoCoinCap>();
            LoadCryptocurrenciesAsync();
        }

        private async Task LoadCryptocurrenciesAsync()
        {
            try
            {
                string url = "https://api.coincap.io/v2/assets?limit="+CryptoViewLimit+"&offset=0";                   
                var response = await _apiService.LoadDataFromApi(url);
                JObject data = JObject.Parse(response);
                JArray cryptos = (JArray)data["data"];
                Cryptocurrencies.Clear();
                foreach (var item in cryptos)
                {
                    CryptoCoinCap crypto = new CryptoCoinCap
                    {
                        Rank = item["rank"].ToString(),
                        Name = item["name"].ToString(),
                        Symbol = item["symbol"].ToString(),
                        PriceUsd = item["priceUsd"].ToString()
                    };

                    Cryptocurrencies.Add(crypto);
                }
            }
            catch (Exception ex) { }
        }

        private void NavigateToDetails(object? cryptoName)
        {
            string? name = cryptoName as string;
            if (!string.IsNullOrEmpty(name))
            {
                var factory = App.ServiceProvider.GetRequiredService<ICryptoDetailsPageFactory>();
                CryptoDetailsPage detailsPage = factory.Create();
                var viewModel = (CryptoDetailsViewModel)detailsPage.DataContext;
                viewModel.previousPage = MethodBase.GetCurrentMethod().DeclaringType.Name.Replace("ViewModel", "");
                viewModel?.LoadCryptoData(name);
                Frame? frame = FindNavigationFrame(viewModel.previousPage);
                frame?.Navigate(detailsPage);
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
