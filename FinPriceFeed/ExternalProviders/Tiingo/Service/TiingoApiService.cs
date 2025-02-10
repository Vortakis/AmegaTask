
using FinPriceFeed.Configuration;
using FinPriceFeed.Domain.Enum;
using FinPriceFeed.Domain.Model;
using FinPriceFeed.ExternalProviders;
using FinPriceFeed.ExternalProviders.Tiingo;
using Microsoft.Extensions.Options;
using Refit;
using System.Diagnostics;

namespace FinPriceFeed.ExternalClients
{
    public class TiingoApiService : IExternalProviderService
    {
        private readonly IExternalProviderClient _tiingoClient;
        private readonly ILogger<TiingoApiService> _logger;

        public TiingoApiService(
            IExternalProviderClient tiingoClient, 
            ILogger<TiingoApiService> logger)
        {
            _tiingoClient = tiingoClient;
            _logger = logger;
        }

        public async Task<List<FinInstrument>> GetFinInstrumentsAsync()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _logger.LogWarning("External Operations: Fetching All Instruments...");

            List<FinInstrument> result = new ();
            var forexTask = _tiingoClient.GetFinInstrumentsAsync<List<ForexFI>>(TiingoEndpoints.Instruments[MarketType.Forex]);
            var cryptoTask = _tiingoClient.GetFinInstrumentsAsync<List<CryptoFI>>(TiingoEndpoints.Instruments[MarketType.Crypto]);
            var stockTask = _tiingoClient.GetFinInstrumentsAsync<List<StockFI>>(TiingoEndpoints.Instruments[MarketType.Stock]);

            await Task.WhenAll(forexTask, cryptoTask, stockTask);

            result.AddRange(await forexTask);
            result.AddRange(await cryptoTask);
            result.AddRange(await stockTask);

            result = result.OrderBy(item => item.Ticker).ToList();

            stopwatch.Stop();
            _logger.LogWarning($"External Operations: Instruments Fetched! Time Elapsed: {stopwatch.Elapsed.TotalSeconds}");

            return result;
        }

        public async Task<List<FinInstrumentPrice>> GetCurrentPriceAsync(string ticker, MarketType type)
        {
            _logger.LogWarning($"External Operations: Fetching Current Price for Ticker: '{ticker}'...");
            Stopwatch stopwatch = Stopwatch.StartNew();

            List<FinInstrumentPrice> result = new();

            var queryParams = new Dictionary<string, string>
            {
                { "tickers", ticker }
            };

            if (type != MarketType.Undefined)
            {
                result = await _tiingoClient.GetCurrentPriceAsync<List<FinInstrumentPrice>>(TiingoEndpoints.Prices[type], queryParams);
            } 
            else
            {
                var tasks = new List<Task<List<FinInstrumentPrice>>>();

                foreach (MarketType market in Enum.GetValues(typeof(MarketType)))
                {
                    if (market == MarketType.Undefined)
                        continue;

                    tasks.Add(_tiingoClient.GetCurrentPriceAsync<List<FinInstrumentPrice>>(TiingoEndpoints.Prices[market], queryParams));
                }
                try
                {
                    var resultsList = await Task.WhenAll(tasks);
                    result = resultsList.SelectMany(r => r).ToList();
                }
                catch (ApiException ex)
                {
                }
            }

            stopwatch.Stop();
            _logger.LogWarning($"External Operations: Current Price Fetched! Time Elapsed: {stopwatch.Elapsed.TotalSeconds}");

            return result;
        }
    }
}
