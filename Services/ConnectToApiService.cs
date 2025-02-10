using System.IO;
using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace CryptoInfo.Services
{
    public class ConnectToApiService
    {
        public string? ApiKey { get; }
        public string? ApiKeyHeader { get; }

        public ConnectToApiService(string apiName)
        {
            string? basePath = Directory.GetCurrentDirectory();
            string? configPath = Path.Combine(basePath, "appsettings.json");

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(configPath, optional: false, reloadOnChange: true)
                .Build();
            if (!string.IsNullOrEmpty(apiName)) {
                ApiKey = config[$"ApiSettings:{apiName}ApiKey"];
                ApiKeyHeader = config[$"ApiSettings:{apiName}ApiKeyHeader"];
            }
        }

        public async Task<string> LoadDataFromApi(string Uri)
        {
            try
            {
                HttpClient _httpClient = new();
                var request = new HttpRequestMessage
                {
                Method = HttpMethod.Get,
                RequestUri = new Uri(Uri)
            };
                if (ApiKey != null)
                {
                    request.Headers.Add("accept", "application/json");
                    request.Headers.Add(ApiKeyHeader, ApiKey);
                }
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        "Network Error. Check connection to the Internet and try again",
                        "Connection Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                });
                return string.Empty;
            }
        }
    }
}
