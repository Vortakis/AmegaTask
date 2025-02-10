using FinPriceFeed.Core.Model;
using FinPriceFeed.Domain.Enum;
using FinPriceFeed.Domain.Model;
using System.Net;

namespace FinPriceFeed.Service
{
    public interface IFinInstrumentService
    {
        Task<PaginatedResult<FinInstrument>> GetAvailableFinInstrumentsAsync(int page, int pageSize);

        Task<List<FinInstrumentPrice>> GetCurrentFinInstrumentPriceAsync(string ticker, MarketType marketType);
    }
}
