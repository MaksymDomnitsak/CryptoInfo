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
    private string? _statusBarText = "";
    private CryptoCoinGecko? _selectedCoin;

    public string? SearchQuery
    {
      get => _searchQuery;
      set
      {
        Set(ref _searchQuery, value);
        if (!string.IsNullOrWhiteSpace(_searchQuery))
        {
          SearchCryptoAsync(_searchQuery);
        }
        else
        {
          Coins.Clear();
        }
      }
    }

    public CryptoCoinGecko? SelectedCoin
    {
      get => _selectedCoin;
      set => Set(ref _selectedCoin, value);
    }

    public string? StatusBarText
    {
      get => _statusBarText;
      set => Set(ref _statusBarText, value);
    }

    public ICommand OpenDetailsCommand { get; }
    public ICommand OpenConverterCommand { get; }
    public ICommand ChangeStatusBarTextCommand { get; }

    public ObservableCollection<CryptoCoinGecko> Coins { get; set; }

    public SearchCryptoViewModel(ConnectToApiService service)
    {
      _apiService = service;
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
      ChangeStatusBarTextCommand = new RelayCommand(ChangeStatusBarText);
      SearchCryptoAsync(SearchQuery!);
    }

    public async void SearchCryptoAsync(string query)
    {
      try
      {
        string uri = $"https://api.coingecko.com/api/v3/search?query={query}";
        var body = await _apiService.LoadDataFromApi(uri, "CoinGecko");
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
    private void NavigateToDetails(object? o)
    {
      if (!string.IsNullOrEmpty(SelectedCoin!.Name))
      {
        var detailsPage = App.ServiceProvider!.GetRequiredService<ICryptoDetailsPageFactory>().Create();
        var viewModel = (CryptoDetailsViewModel)detailsPage.DataContext;
        viewModel.PreviousPage = MethodBase.GetCurrentMethod()!.DeclaringType!.Name.Replace("ViewModel", "");
        viewModel?.LoadCryptoData(SelectedCoin!.Name);
        var parentDataContext = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
        parentDataContext.saveSearch = SearchQuery!;
        Frame? frame = NavigationHelper.FindNavigationFrame(viewModel!.PreviousPage);
        frame?.Navigate(detailsPage);
      }
    }

    private void NavigateToConverter(object? o)
    {
      if (!string.IsNullOrEmpty(SelectedCoin!.Id))
      {
        var converterPage = App.ServiceProvider!.GetRequiredService<ICurrencyConvertPageFactory>().Create();
        var viewModel = (CurrencyConvertViewModel)converterPage.DataContext;
        viewModel.SelectedCrypto = SelectedCoin!.Id;
        viewModel.PreviousPage = MethodBase.GetCurrentMethod()!.DeclaringType!.Name.Replace("ViewModel", "");
        Frame? frame = NavigationHelper.FindNavigationFrame(viewModel.PreviousPage);
        var parentDataContext = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
        parentDataContext.saveSearch = SearchQuery!;
        frame?.Navigate(converterPage);
      }
    }

    private void ChangeStatusBarText(object? value)
    {
      string? val = value as string;
      if (String.IsNullOrEmpty(val)) StatusBarText = "";
      else StatusBarText = val;
    }
  }
}
