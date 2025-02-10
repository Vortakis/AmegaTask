using FinPriceFeed.Core.Model;
using FinPriceFeed.Domain.Enum;
using FinPriceFeed.Domain.Model;
using FinPriceFeed.ExternalClients;
using Microsoft.Extensions.Caching.Memory;

namespace FinPriceFeed.Service
{
    public class FinInstrumentService : IFinInstrumentService
    {
        private readonly IExternalProviderService _extProviderService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<FinInstrumentService> _logger;

        public FinInstrumentService(IExternalProviderService externalProviderService, IMemoryCache cache, ILogger<FinInstrumentService> logger)
        {
            _extProviderService = externalProviderService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<PaginatedResult<FinInstrument>> GetAvailableFinInstrumentsAsync(int page, int pageSize)
        {
            List<FinInstrument> result;
            if (_cache.TryGetValue("finInstruments", out result))
            {
                _logger.LogWarning("Returning cached instruments.");
            }
            else
            {
                result = await _extProviderService.GetFinInstrumentsAsync();
                if (result != null && result.Count > 0)
                {
                    _cache.Set("finInstruments", result);
                }
            }

            var paginatedResult = result.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var response = new PaginatedResult<FinInstrument>(paginatedResult, result.Count, paginatedResult.Count);

            return response;
        }

        public async Task<List<FinInstrumentPrice>> GetCurrentFinInstrumentPriceAsync(string ticker, MarketType marketType)
        {
            var result = await _extProviderService.GetCurrentPriceAsync(ticker, marketType);

            return result;
        }
    }
}
