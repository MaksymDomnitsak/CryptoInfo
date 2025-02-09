namespace CryptoInfo.Services
{
    public class ConnectToApiServiceFactory
    {
        public ConnectToApiService Create(string apiName = "") => new ConnectToApiService(apiName);
    }
}
