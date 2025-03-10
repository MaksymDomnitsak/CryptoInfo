using CryptoInfo.Services.Interfaces;
using CryptoInfo.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoInfo.Services
{
  class CryptoDetailsPageFactory : ICryptoDetailsPageFactory
  {
    private readonly IServiceProvider _serviceProvider;

    public CryptoDetailsPageFactory(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    public CryptoDetailsPage Create()
    {
      return _serviceProvider.GetRequiredService<CryptoDetailsPage>();
    }
  }
}
