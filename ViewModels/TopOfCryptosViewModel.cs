using System.Collections.ObjectModel;
using System.Net.Http;
using CryptoInfo.Models;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using CryptoInfo.Views;
using CryptoInfo.Helpers.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Reflection;

namespace CryptoInfo.ViewModels
{
    internal class TopOfCryptosViewModel : BaseViewModel
    {
        private ObservableCollection<CryptoCoinCap> _cryptocurrencies;
        public ObservableCollection<CryptoCoinCap> Cryptocurrencies
        {
            get => _cryptocurrencies;
            set => Set(ref _cryptocurrencies, value);
        }

        private int _cryptoViewLimit = 20;

        public int CryptoViewLimit
        {
            get => _cryptoViewLimit;
            set => Set(ref _cryptoViewLimit, value);
        }

        public ICommand NavigateToDetailsCommand { get; }

        public TopOfCryptosViewModel() 
        {
            NavigateToDetailsCommand = new RelayCommand(NavigateToDetails);
            Cryptocurrencies = new ObservableCollection<CryptoCoinCap>();
            LoadCryptocurrenciesAsync();
        }

        private async Task LoadCryptocurrenciesAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://api.coincap.io/v2/assets?limit="+CryptoViewLimit+"&offset=0";

                    var response = await client.GetStringAsync(url);
                    JObject data = JObject.Parse(response);
                    JArray cryptos = (JArray)data["data"];

                    Cryptocurrencies.Clear();
                    foreach (var item in cryptos)
                    {
                        CryptoCoinCap crypto = new CryptoCoinCap
                        {
                            Id = item["id"].ToString(),
                            Name = item["name"].ToString(),
                            Symbol = item["symbol"].ToString(),
                            Supply = item["supply"].ToString(),
                            PriceUsd = item["priceUsd"].ToString()
                        };

                        Cryptocurrencies.Add(crypto);
                    }

                }
            }
            catch (Exception ex) { }
        }

        private void NavigateToDetails(object? cryptoName)
        {
            string? name = cryptoName as string;
            if (!string.IsNullOrEmpty(name))
            {
                var detailsPage = new CryptoDetailsPage();
                var viewModel = (CryptoDetailsViewModel)detailsPage.DataContext;
                viewModel.previousPage = MethodBase.GetCurrentMethod().DeclaringType.Name.Replace("ViewModel","");
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
}
