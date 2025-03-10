using System.Windows;
using CryptoInfo.Services;
using CryptoInfo.Services.Interfaces;
using CryptoInfo.ViewModels;
using CryptoInfo.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoInfo
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public static IServiceProvider? ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      var services = new ServiceCollection();
      ConfigureServices(services);
      ServiceProvider = services.BuildServiceProvider();

      var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
      mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton<ConnectToApiService>();

      services.AddTransient<MainWindow>();
      services.AddTransient<MainWindowViewModel>();
      services.AddTransient<TopOfCryptosPage>();
      services.AddTransient<TopOfCryptosViewModel>();
      services.AddTransient<SearchCryptoPage>();
      services.AddTransient<SearchCryptoViewModel>();
      services.AddTransient<CryptoDetailsPage>();
      services.AddTransient<CryptoDetailsViewModel>();
      services.AddTransient<CurrencyConvertPage>();
      services.AddTransient<CurrencyConvertViewModel>();
      services.AddTransient<CandlestickChartViewModel>();
      services.AddTransient<CandlestickChartUserControl>();
      services.AddTransient<ICurrencyConvertPageFactory, CurrencyConvertPageFactory>();
      services.AddTransient<ICryptoDetailsPageFactory, CryptoDetailsPageFactory>();
    }
  }

}
