using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using CryptoInfo.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoInfo.ViewModels
{
    internal class TopOfCryptosViewModel : BaseViewModel
    {
        private ObservableCollection<Cryptocurrency> _cryptocurrencies;
        public ObservableCollection<Cryptocurrency> Cryptocurrencies
        {
            get => _cryptocurrencies;
            set
            {
                _cryptocurrencies = value;
                OnPropertyChanged(nameof(Cryptocurrencies));
            }
        }

        private int _cryptoViewLimit = 20;

        public int CryptoViewLimit
        {
            get => _cryptoViewLimit;
            set
            {
                _cryptoViewLimit = value;
                OnPropertyChanged(nameof(CryptoViewLimit));
            }
        }

        public TopOfCryptosViewModel() 
        {
            Cryptocurrencies = new ObservableCollection<Cryptocurrency>();
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
                        Cryptocurrency crypto = new Cryptocurrency
                        {
                            Name = item["name"].ToString(),
                            Symbol = item["symbol"].ToString(),
                            MarketCapUsd = item["marketCapUsd"].ToString(),
                            Supply = item["supply"].ToString(),
                            PriceUsd = item["priceUsd"].ToString()
                        };

                        Cryptocurrencies.Add(crypto);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
        }
    }
}
