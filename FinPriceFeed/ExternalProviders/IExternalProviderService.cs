using FinPriceFeed.Domain.Enum;
using FinPriceFeed.Domain.Model;

namespace FinPriceFeed.ExternalClients
{
    public interface IExternalProviderService
    {
        Task<List<FinInstrument>> GetFinInstrumentsAsync();

        Task<List<FinInstrumentPrice>> GetCurrentPriceAsync(string tickers, MarketType type);

    }
}
