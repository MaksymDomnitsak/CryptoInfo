using CryptoInfo.Helpers.Commands;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using CryptoInfo.Models;
using CryptoInfo.Services;
using System.Text.RegularExpressions;
using CryptoInfo.Helpers;

namespace CryptoInfo.ViewModels
{
  public class CurrencyConvertViewModel : BaseViewModel
  {
    private readonly ConnectToApiService _service;
    private string? _selectedCrypto;
    private string? _selectedFiat;
    private string _amount = "";
    private string? _convertedAmount;
    private Dictionary<string, decimal> _priceDictionary = [];

    private string? _previousPage = "";
    public ObservableCollection<CryptoSymbol> Cryptos { get; set; } = [];
    public ObservableCollection<string> FiatCurrencies { get; set; } = [];

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
        value = Regex.Replace(value, @"[^0-9,]", "");
        if (value.IndexOf(',') != -1 && value.IndexOf(',') != value.LastIndexOf(',')) return;
        if (value.IndexOf(',') != -1 && value.Length - value.IndexOf(',') - 1 > 20)
        {
          value = value[..(value.IndexOf(',') + 21)];
        }
        if (!string.IsNullOrEmpty(ConvertedAmount) && ConvertedAmount.StartsWith("> 79228162514264337593543950335"))
        {
          if (value.Length < _amount.Length)
          {
            _amount = value;
          }
        }
        else
        {
          _amount = value;
        }
        OnPropertyChanged(nameof(Amount));
        ConvertCurrency();
      }
    }

    public string? ConvertedAmount
    {
      get => _convertedAmount;
      set => Set(ref _convertedAmount, value);
    }

    public string? PreviousPage
    {
      get => _previousPage;
      set => Set(ref _previousPage, value);
    }

    public ICommand GoBackCommand { get; }

    public CurrencyConvertViewModel(ConnectToApiService service)
    {
      _service = service;
      GoBackCommand = new RelayCommand(NavigateCommands.GoBack, NavigateCommands.CanGoBack);
      LoadData();
    }

    private async void LoadData()
    {
      try
      {
        string uri = "https://api.coingecko.com/api/v3/simple/supported_vs_currencies";
        var body = await _service.LoadDataFromApi(uri, "CoinGecko");
        var fiatList = JsonSerializer.Deserialize<List<string>>(body);
        FiatCurrencies.Clear();
        foreach (var fiat in fiatList!)
        {
          FiatCurrencies.Add(fiat.ToUpper());
        }
      }
      catch (Exception ex)
      {
        GeneralMessageBoxForException.Invoke(ex.Message);
      }
    }

    private async void FetchCryptoPrice()
    {
      try
      {
        string uri = $"https://api.coingecko.com/api/v3/coins/{SelectedCrypto}?localization=false" +
            $"&tickers=false&market_data=true&community_data=false&developer_data=false";
        var body = await _service.LoadDataFromApi(uri, "CoinGecko");
        JObject cryptoInfo = JObject.Parse(body);
        var current_price = cryptoInfo["market_data"]?["current_price"] as JObject;
        _priceDictionary = current_price?.Properties().ToDictionary(
            prop => prop.Name,
            prop => prop.Value.Value<decimal>()
        ) ?? new Dictionary<string, decimal>();
        ConvertCurrency();
      }
      catch (Exception ex)
      {
        GeneralMessageBoxForException.Invoke(ex.Message);
      }
    }

    private void ConvertCurrency()
    {
      try
      {
        if (_priceDictionary != null && SelectedFiat != null && _priceDictionary.ContainsKey(SelectedFiat.ToLower()))
        {
          ConvertedAmount = ((String.IsNullOrEmpty(Amount) ? 0 : Decimal.Parse(Amount))
              * _priceDictionary[SelectedFiat.ToLower()]).ToString() + " " + SelectedFiat;
        }
        else
        {
          ConvertedAmount = "0";
        }
      }
      catch (Exception)
      {
        ConvertedAmount = "> 79228162514264337593543950335 " + SelectedFiat;
      }
    }
  }
}
