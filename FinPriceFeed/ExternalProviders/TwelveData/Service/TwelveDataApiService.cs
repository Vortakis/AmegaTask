
using AutoMapper;
using FinPriceFeed.Domain.Enum;
using FinPriceFeed.Domain.Model;
using FinPriceFeed.ExternalProviders;
using FinPriceFeed.ExternalProviders.TwelveData;
using FinPriceFeed.ExternalProviders.TwelveData.DTOs;
using System.Diagnostics;

namespace FinPriceFeed.ExternalClients
{
    public class TwelveDataApiService : IExternalProviderService
    {
        private readonly IExternalProviderClient _twelveDataClient;
        private readonly IMapper _mapper;
        private readonly ILogger<TwelveDataApiService> _logger;

        public TwelveDataApiService(
            IExternalProviderClient twelveDataClient,
            ILogger<TwelveDataApiService> logger,
            IMapper mapper)
        {
            _twelveDataClient = twelveDataClient;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<FinInstrument>> GetFinInstrumentsAsync()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _logger.LogWarning("External Operations: Fetching All Instruments...");

            var forexTask = _twelveDataClient.GetFinInstrumentsAsync<TDForexFI>(TwelveDataEndpoints.Instruments[MarketType.Forex]);
            var cryptoTask = _twelveDataClient.GetFinInstrumentsAsync<TDCryptoFI>(TwelveDataEndpoints.Instruments[MarketType.Crypto]);
            var stockTask = _twelveDataClient.GetFinInstrumentsAsync<TDStockFI>(TwelveDataEndpoints.Instruments[MarketType.Stock]);

            await Task.WhenAll(forexTask, cryptoTask, stockTask);

            var combinedData = forexTask.Result.Data
                .Concat(cryptoTask.Result.Data)
                .Concat(stockTask.Result.Data)
                .OrderBy(item => item.Symbol)
                .ToList();

            var result = _mapper.Map<List<FinInstrument>>(combinedData);

            stopwatch.Stop();
            _logger.LogWarning($"External Operations: Instruments Fetched! Time Elapsed: {stopwatch.Elapsed.TotalSeconds}");

            return result;
        }

        public async Task<List<FinInstrumentPrice>> GetCurrentPriceAsync(string ticker, MarketType type)
        {
            _logger.LogWarning($"External Operations: Fetching Current Price for Ticker: '{ticker}'...");
            Stopwatch stopwatch = Stopwatch.StartNew();

            var queryParams = new Dictionary<string, string>
            {
                { "symbol", ticker }
            };

            var result = await _twelveDataClient.GetCurrentPriceAsync<FinInstrumentPrice>(TwelveDataEndpoints.Price, queryParams);

            stopwatch.Stop();
            _logger.LogWarning($"External Operations: Current Price Fetched! Time Elapsed: {stopwatch.Elapsed.TotalSeconds}");

            List<FinInstrumentPrice> priceList = new();
            priceList.Add(result);

            return priceList;
        }
    }
}
