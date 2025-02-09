using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System.Collections.ObjectModel;
using System.Text.Json;
using CryptoInfo.Services;

namespace CryptoInfo.ViewModels
{
    public class CandlestickChartViewModel: BaseViewModel
    {
        private PlotModel? _plotModel;
        private string _selectedInterval = "1";
        private string? _selectedCryptoId;
        private ConnectToApiService _service;

        public ObservableCollection<string> IntervalOptions { get; } = new ObservableCollection<string> { "1", "7", "14" };

        public string SelectedInterval
        {
            get => _selectedInterval;
            set
            {
                if (_selectedInterval != value)
                {
                    Set(ref _selectedInterval, value);
                    LoadChartData();
                }
            }
        }

        public string? SelectedCryptoId
        {
            get => _selectedCryptoId;
            set
            {
                Set(ref _selectedCryptoId, value);
                LoadChartData();
            }
        }

        public PlotModel? PlotModel
        {
            get => _plotModel;
            set => Set(ref _plotModel, value);
        }

        public CandlestickChartViewModel(ConnectToApiServiceFactory factory)
        {
            _service = factory.Create("CoinGecko");
        }

        public async void LoadChartData()
        {
            string uri = $"https://api.coingecko.com/api/v3/coins/{SelectedCryptoId}/ohlc?vs_currency=usd&days={SelectedInterval}&precision=5";
            try
            {
                string json = await _service.LoadDataFromApi(uri);
                var ohlcData = JsonSerializer.Deserialize<double[][]>(json);

                if (ohlcData != null)
                {
                    var newModel = new PlotModel { Title = $"{SelectedCryptoId} - Candlestick Chart" };

                    var dateAxis = new DateTimeAxis
                    {
                        Position = AxisPosition.Bottom,
                        StringFormat = "MM/dd HH:mm",
                        Title = "Time",
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot
                    };

                    var priceAxis = new LinearAxis
                    {
                        Position = AxisPosition.Left,
                        Title = "Price (USD)",
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot
                    };

                    newModel.Axes.Add(dateAxis);
                    newModel.Axes.Add(priceAxis);

                    var candleSeries = new CandleStickSeries { IncreasingColor = OxyColors.Green, DecreasingColor = OxyColors.Red };

                    foreach (var entry in ohlcData)
                    {
                        var time = DateTimeOffset.FromUnixTimeMilliseconds((long)entry[0]).DateTime;
                        candleSeries.Items.Add(new HighLowItem(time.ToOADate(), entry[2], entry[3], entry[1], entry[4]));
                    }

                    newModel.Series.Add(candleSeries);
                    PlotModel = newModel;
                }
                else if(ohlcData.Length==0)
                {
                    var dateAxis = new DateTimeAxis
                    {
                        Position = AxisPosition.Bottom,
                        StringFormat = "MM/dd HH:mm",
                        Title = "Time",
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot
                    };

                    var priceAxis = new LinearAxis
                    {
                        Position = AxisPosition.Left,
                        Title = "Price (USD)",
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot
                    };
                    var newModel = new PlotModel { Title = $"{SelectedCryptoId} - Candlestick Chart" };
                    newModel.Axes.Add(dateAxis);
                    newModel.Axes.Add(priceAxis);
                    PlotModel = newModel;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
