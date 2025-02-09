using CryptoInfo.Services.Interfaces;
using CryptoInfo.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoInfo.Services
{
    public class CurrencyConvertPageFactory : ICurrencyConvertPageFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CurrencyConvertPageFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public CurrencyConvertPage Create()
        {
            return _serviceProvider.GetRequiredService<CurrencyConvertPage>();
        }
    }
}
