using System.IO;
using System.Net.Http;
using System.Windows;
using CryptoInfo.Helpers;
using Microsoft.Extensions.Configuration;

namespace CryptoInfo.Services
{
  public class ConnectToApiService
  {
    private static readonly HttpClient? _httpClient = new();
    private readonly IConfiguration _config;

    public ConnectToApiService()
    {
      string? basePath = Directory.GetCurrentDirectory();
      string? configPath = Path.Combine(basePath, "appsettings.json");

      _config = new ConfigurationBuilder()
          .SetBasePath(basePath)
          .AddJsonFile(configPath, optional: false, reloadOnChange: true)
          .Build();

    }

    public async Task<string> LoadDataFromApi(string uri, string apiName = "")
    {
      try
      {
        string ApiKey = "", ApiKeyHeader = "";
        if (!String.IsNullOrEmpty(apiName))
        {
          ApiKey = _config[$"ApiSettings:{apiName}ApiKey"]!;
          ApiKeyHeader = _config[$"ApiSettings:{apiName}ApiKeyHeader"]!;
        }
        var request = new HttpRequestMessage
        {
          Method = HttpMethod.Get,
          RequestUri = new Uri(uri)
        };
        if (!String.IsNullOrEmpty(ApiKey) && !String.IsNullOrEmpty(ApiKeyHeader))
        {
          request.Headers.Add("accept", "application/json");
          request.Headers.Add(ApiKeyHeader, ApiKey);
        }
        var response = (await _httpClient!.SendAsync(request)).EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
      }
      catch
      {
        GeneralMessageBoxForException.Invoke("Network Error. Check connection to the Internet and try again", "Connection Error");
        return string.Empty;
      }
    }
  }
}
